using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using vBase.Core.DTOs;
using vBase.Core.Exceptions;
using vBase.Core.Utilities;

namespace vBase.Core.Web3CommitmentService;


/// <summary>
/// Provides access to the CommitmentService smart contract.
/// </summary>
public abstract class Web3CommitmentService: ICommitmentService
{
  private readonly Contract _commitmentServiceContract;
  protected readonly Account Account;
  private readonly Web3 _web3;

  public Web3CommitmentService(string privateKey)
  {
    if (string.IsNullOrWhiteSpace(privateKey))
      throw new ArgumentException("Private key is required.", nameof(privateKey));

    Account = new Account(privateKey);
    _web3 = new Web3(Account);
    var contractDefinitionJson = Utils.LoadEmbeddedJson("CommitmentService.json");
    _commitmentServiceContract = _web3.Eth.GetContract(contractDefinitionJson, "0x1234");
  }

  public string AccountIdentifier => Account.Address.ConvertToEthereumChecksumAddress();

  /// <summary>
  /// Executes Smart Contract function.
  /// </summary>
  /// <param name="function">Function descriptor.</param>
  /// <param name="functionData">Data which will be passed as a function arguments.</param>
  /// <returns></returns>
  protected abstract Task<ReceiptDto<ContractMethodExecuteResultDto>> CallContractFunction(Function function, string functionData);

  /// <summary>
  /// Fetches state variable from the Smart Contract.
  /// </summary>
  /// <typeparam name="TResultType">Expected result type</typeparam>
  /// <param name="functionData">Encoded state variable</param>
  /// <returns>Variable value</returns>
  protected abstract Task<ReceiptDto<TResultType>> FetchStateVariable<TResultType>(string functionData);

  /// <summary>
  /// Checks if the specified set exists.
  /// </summary>
  /// <param name="owner">User set owner.</param>
  /// <param name="setName">Name of the set.</param>
  public async Task<bool> UserSetExists(string owner, string setName)
  {
    var res = await CallStateVariable<string>("userSetCommitments", owner, setName.GetCid().Data);
    int parsedRes = (int)(new Int32Converter()).ConvertFromString(res);
    return parsedRes == 1;
  }

  /// <summary>
  /// Checks whether the object with the specified CID was stamped at the given time.
  /// </summary>
  /// <param name="owner">Object owner.</param>
  /// <param name="objectCid">The CID of the object.</param>
  /// <param name="timestamp">The timestamp of the transaction.</param>
  /// <returns>True if the object was stamped, otherwise false.</returns>
  public async Task<bool> VerifyUserObject(string owner, Cid objectCid, DateTimeOffset timestamp)
  {
    var res = await CallStateVariable<string>("verifyUserObject", owner, objectCid.Data, timestamp.ToUnixTimeSeconds());
    int parsedRes = (int)(new Int32Converter()).ConvertFromString(res);
    return parsedRes == 1;
  }

  public async Task<bool> VerifyUserSetObjects(string owner, Cid setCid, BigInteger setObjectsCidSum)
  {
    var res = await CallStateVariable<string>("verifyUserSetObjectsCidSum", owner, setCid.Data, setObjectsCidSum);
    int parsedRes = (int)(new Int32Converter()).ConvertFromString(res);
    return parsedRes == 1;
  }

  /// <summary>
  /// Creates a new data set with the specified set name.
  /// If the set already exists, no action will be taken.
  /// </summary>
  /// <param name="setName">The name of the new set.</param>
  public async Task AddSet(string setName)
  {
    var setNameCid = setName.GetCid();
    var contractMethodExecutionRes = await CallContractFunction("addSet", setNameCid.Data);
    var addSetEvents = _commitmentServiceContract.GetEvent("AddSet")
      .DecodeAllEventsDefaultForEvent(contractMethodExecutionRes.Logs);

    // of no event is found, the set already exists
    var operationEvent = addSetEvents.SingleOrDefault();

    if (operationEvent != null)
    {
      // a new set has been created, let's do some crosscheck
      OperationEventCrossCheckUserAddress(operationEvent);
      OperationEventCrossCheckSetCid(operationEvent, setNameCid);
    }
  }

  /// <summary>
  /// Adds a objectToAdd to the specified set.
  /// </summary>
  /// <param name="setName">Name of the set where the objectToAdd will be added.</param>
  /// <param name="cidToAdd">Object CID to add.</param>
  public async Task<DateTimeOffset> AddSetObject(string setName, Cid cidToAdd)
  {
    var setNameCid = setName.GetCid();
    var contractMethodExecutionRes = await CallContractFunction("addSetObject", setNameCid.Data, cidToAdd.Data);
    var operationsEvents = _commitmentServiceContract.GetEvent("AddSetObject")
      .DecodeAllEventsDefaultForEvent(contractMethodExecutionRes.Logs);

    if (!operationsEvents.Any())
    {
      // no events emitted - the objectToAdd has not been added
      throw new vBaseException($"Failed to add a objectToAdd to set '{setName}'. Please make sure that the set with the specified name exists.");
    }

    EventLog<List<ParameterOutput>> operationEvent = operationsEvents.Single();

    // object added, let's do some crosscheck
    OperationEventCrossCheckUserAddress(operationEvent);
    OperationEventCrossCheckSetCid(operationEvent, setNameCid);
    OperationEventCrossCheckAddedRecordCid(operationEvent, cidToAdd);

    return DateTimeOffset.FromUnixTimeSeconds((long)operationEvent.GetEventParameterValue<BigInteger>("timestamp"));
  }

  /// <summary>
  /// Calls the specified contract function.
  /// </summary>
  /// <param name="functionName">The name of the function to call.</param>
  /// <param name="functionInput">The input parameters for the function.</param>
  /// <returns>The result of the contract function execution.</returns>
  private async Task<ContractMethodExecuteResultDto> CallContractFunction(string functionName, params object[] functionInput)
  {
    var function = _commitmentServiceContract.GetFunction(functionName);
    var functionData = function.GetData(functionInput);
    var receipt = await CallContractFunction(function, functionData);

    if (!receipt.Success)
      throw new vBaseException($"Failed to call a contract function {functionName}");

    return receipt.Data;
  }

  /// <summary>
  /// Fetches the specified state variable from the contract.
  /// </summary>
  /// <typeparam name="TResultType">Expected variable type</typeparam>
  /// <param name="stateVariableName">Name of the variable to fetch</param>
  /// <param name="functionInput">Context identifying the set</param>
  /// <returns>Variable value</returns>
  private async Task<TResultType> CallStateVariable<TResultType>(string stateVariableName, params object[] functionInput)
  {
    var function = _commitmentServiceContract.GetFunction(stateVariableName);
    var functionData = function.GetData(functionInput);
    var receipt = await FetchStateVariable<TResultType>(functionData);

    if (!receipt.Success)
      throw new vBaseException($"Failed to call a contract state variable {stateVariableName}");

    return receipt.Data;
  }

  private void OperationEventCrossCheckUserAddress(EventLog<List<ParameterOutput>> operationEvent)
  {
    if (operationEvent.GetEventParameterValue<string>("user") != Account.Address.ConvertToEthereumChecksumAddress())
      throw new vBaseException("The user address in the event does not match the account address");
  }

  private void OperationEventCrossCheckSetCid(EventLog<List<ParameterOutput>> operationEvent, Cid setNameCid)
  {
    if (!operationEvent.GetEventParameterValue<byte[]>("setCid").SequenceEqual(setNameCid.Data))
      throw new vBaseException("The set CID in the event does not match the requested set CID");
  }

  private void OperationEventCrossCheckAddedRecordCid(EventLog<List<ParameterOutput>> operationEvent, Cid addedObjectCid)
  {
    if (!operationEvent.GetEventParameterValue<byte[]>("objectCid").SequenceEqual(addedObjectCid.Data))
      throw new vBaseException("The object CID in the event does not match the added object CID");
  }
}