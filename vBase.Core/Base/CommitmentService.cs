using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using vBase.Core.DTOs;
using vBase.Core.Exceptions;
using vBase.Core.Utilities;

namespace vBase.Core.Base;

public class CommitmentService
{
  private readonly Contract _commitmentServiceContract;
  private readonly ICommunicationChannel _communicationChannel;
  private readonly Account _account;
  private readonly Web3 _web3;

  public CommitmentService(ICommunicationChannel communicationChannel, string privateKey)
  {
    _communicationChannel = communicationChannel;
    _account = new Account(privateKey);
    _web3 = new Web3(_account);
    var contractDefinitionJson = Utils.LoadEmbeddedJson("CommitmentService.json");
    _commitmentServiceContract = _web3.Eth.GetContract(contractDefinitionJson, "0x1234");
  }

  /// <summary>
  /// Checks if the specified set exists.
  /// </summary>
  /// <param name="setName">Name of the set.</param>
  public async Task<bool> UserSetExists(string setName)
  {
    var res = await CallStateVariable<string>("userSetCommitments", _account.ChecksumAddress(), setName.GetCid());
    int parsedRes = (int)(new Int32Converter()).ConvertFromString(res);
    return parsedRes == 1;
  }

  /// <summary>
  /// Checks whether the object with the specified CID was stamped at the given time.
  /// </summary>
  /// <param name="objectCid">The CID of the object.</param>
  /// <param name="timestamp">The timestamp of the transaction.</param>
  /// <returns>True if the object was stamped, otherwise false.</returns>
  public async Task<bool> VerifyUserObject(byte[] objectCid, DateTimeOffset timestamp)
  {
    var res = await CallStateVariable<string>("verifyUserObject", _account.ChecksumAddress(), objectCid, timestamp.ToUnixTimeSeconds());
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
    var contractMethodExecutionRes = await CallContractFunction("addSet", setNameCid);
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
  /// <param name="objectToAdd">Object to add.</param>
  public async Task<DateTimeOffset> AddSetObject(string setName, object objectToAdd)
  {
    var setNameCid = setName.GetCid();
    var objectCid = objectToAdd.GetCid();
    var contractMethodExecutionRes = await CallContractFunction("addSetObject", setNameCid, objectCid);
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
    OperationEventCrossCheckAddedRecordCid(operationEvent, objectCid);

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
    var receipt = await _communicationChannel.CallContractFunction(function, functionData);

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
    var receipt = await _communicationChannel.FetchStateVariable<TResultType>(functionData);

    if (!receipt.Success)
      throw new vBaseException($"Failed to call a contract state variable {stateVariableName}");

    return receipt.Data;
  }

  private void OperationEventCrossCheckUserAddress(EventLog<List<ParameterOutput>> operationEvent)
  {
    if (operationEvent.GetEventParameterValue<string>("user") != _account.ChecksumAddress())
      throw new vBaseException("The user address in the event does not match the account address");
  }

  private void OperationEventCrossCheckSetCid(EventLog<List<ParameterOutput>> operationEvent, byte[] setNameCid)
  {
    if (!operationEvent.GetEventParameterValue<byte[]>("setCid").SequenceEqual(setNameCid))
      throw new vBaseException("The set CID in the event does not match the requested set CID");
  }

  private void OperationEventCrossCheckAddedRecordCid(EventLog<List<ParameterOutput>> operationEvent, byte[] addedObjectCid)
  {
    if (!operationEvent.GetEventParameterValue<byte[]>("objectCid").SequenceEqual(addedObjectCid))
      throw new vBaseException("The object CID in the event does not match the added object CID");
  }
}