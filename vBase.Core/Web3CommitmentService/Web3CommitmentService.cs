using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
  protected readonly ILogger Logger;

  public Web3CommitmentService(string privateKey, ILogger logger)
  {
    if (string.IsNullOrWhiteSpace(privateKey))
      throw new ArgumentException("Private key is required.", nameof(privateKey));

    Logger = logger;

    try
    {
      Account = new Account(privateKey);
    }
    catch (Exception ex)
    {
      throw new vBaseException("Failed to create an account from the provided private key.", ex);
    }

    var web3 = new Web3(Account);
    var contractDefinitionJson = Utils.LoadEmbeddedJson("CommitmentService.json");
    _commitmentServiceContract = web3.Eth.GetContract(contractDefinitionJson, "0x1234");
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

  public async Task<bool> UserSetExists(string user, Cid setCid)
  {
    Logger.LogInformation($"Checking if user '{user}' has a set with CID '{setCid.ToHex()}'");

    var res = await CallStateVariable<string>("userSetCommitments", user, setCid.Data);
    int parsedRes = (int)(new Int32Converter()).ConvertFromString(res);
    return parsedRes == 1;
  }

  public async Task<bool> VerifyUserObject(string user, Cid objectCid, DateTimeOffset timestamp)
  {
    Logger.LogInformation($"Verifying user '{user}' object with CID '{objectCid.ToHex()}'");

    var res = await CallStateVariable<string>("verifyUserObject", user, objectCid.Data, timestamp.ToUnixTimeSeconds());
    int parsedRes = (int)(new Int32Converter()).ConvertFromString(res);
    return parsedRes == 1;
  }

  public async Task<bool> VerifyUserSetObjects(string user, Cid setCid, BigInteger setObjectsCidSum)
  {
    Logger.LogInformation($"Verifying user '{user}' set with CID '{setCid.ToHex()}'");

    var res = await CallStateVariable<string>("verifyUserSetObjectsCidSum", user, setCid.Data, setObjectsCidSum);
    int parsedRes = (int)(new Int32Converter()).ConvertFromString(res);
    return parsedRes == 1;
  }

  public async Task AddSet(Cid setCid)
  {
    Logger.LogInformation($"Adding a set with CID '{setCid.ToHex()}'");

    var contractMethodExecutionRes = await CallContractFunction("addSet", setCid.Data);
    var addSetEvents = _commitmentServiceContract.GetEvent("AddSet")
      .DecodeAllEventsDefaultForEvent(contractMethodExecutionRes.Logs);

    // of no event is found, the set already exists
    var operationEvent = addSetEvents.SingleOrDefault();

    if (operationEvent != null)
    {
      // a new set has been created, let's do some crosscheck
      OperationEventCrossCheckUserAddress(operationEvent);
      OperationEventCrossCheckSetCid(operationEvent, setCid);
    }
  }

  public async Task<Receipt> AddSetObject(Cid setCid, Cid objectCid)
  {
    Logger.LogInformation($"Adding object with CID '{objectCid.ToHex()}' to a set with CID '{setCid.ToHex()}'");

    var contractMethodExecutionRes = await CallContractFunction("addSetObject", setCid.Data, objectCid.Data);

    Logger.LogInformation(
      $"Response status: {contractMethodExecutionRes.Status}. Response logs: {contractMethodExecutionRes.Logs}");

    var operationsEvents = _commitmentServiceContract.GetEvent("AddSetObject")
      .DecodeAllEventsDefaultForEvent(contractMethodExecutionRes.Logs);

    if (!operationsEvents.Any())
    {
      // no events emitted - the objectToAdd has not been added
      throw new vBaseException($"Failed to add a objectToAdd a set with CID '{setCid.ToHex()}'. Please make sure that the set with the specified name exists.");
    }

    EventLog<List<ParameterOutput>> operationEvent = operationsEvents.Single();

    // object added, let's do some crosscheck
    OperationEventCrossCheckUserAddress(operationEvent);
    OperationEventCrossCheckSetCid(operationEvent, setCid);
    OperationEventCrossCheckAddedRecordCid(operationEvent, objectCid);

    var timestamp= DateTimeOffset.FromUnixTimeSeconds((long)operationEvent.GetEventParameterValue<BigInteger>("timestamp"));
    return new Web3Receipt(contractMethodExecutionRes.TransactionHash, timestamp);
  }

  /// <summary>
  /// Calls the specified contract function.
  /// </summary>
  /// <param name="functionName">The name of the function to call.</param>
  /// <param name="functionInput">The input parameters for the function.</param>
  /// <returns>The result of the contract function execution.</returns>
  private async Task<ContractMethodExecuteResultDto> CallContractFunction(string functionName, params object[] functionInput)
  {
    Logger.LogInformation($"Calling a contract function {functionName}");

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
    Logger.LogInformation($"Fetching a contract state variable {stateVariableName}");

    var function = _commitmentServiceContract.GetFunction(stateVariableName);
    var functionData = function.GetData(functionInput);
    var receipt = await FetchStateVariable<TResultType>(functionData);

    Logger.LogInformation($"Receipt status: {receipt.Success}");

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