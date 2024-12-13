using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

  public async Task<Dictionary<string, string>> AddSetObject(byte[] setCid, byte[] recordCid)
  {
    var contractMethodExecutionRes = await CallContractFunction("addSetObject", setCid, recordCid);
    return new Dictionary<string, string>();
  }

  public async Task<bool> UserSetExists(byte[] setCid)
  {
    var res = await CallStateVariable<string>("userSetCommitments", _account.ChecksumAddress(), setCid);
    int parsedRes = (int)(new System.ComponentModel.Int32Converter()).ConvertFromString(res);
    return parsedRes == 1;
  }

  /// <summary>
  /// Creates a new data set with the specified setCid.
  /// If the set already exists, no action will be taken.
  /// </summary>
  /// <param name="setCid">The CID representing the name of the new set.</param>
  public async Task AddSet(byte[] setCid)
  {
    var contractMethodExecutionRes = await CallContractFunction("addSet", setCid);
    var addSetEvents = _commitmentServiceContract.GetEvent("AddSet")
      .DecodeAllEventsDefaultForEvent(contractMethodExecutionRes.Logs);

    // of no event is found, the set already exists
    var setCreationEvent = addSetEvents.SingleOrDefault();

    if (setCreationEvent != null)
    {
      // a new set has been created
      string userAddress = setCreationEvent.GetEventParameterValue<string>("user");
      byte[] newSetCid = setCreationEvent.GetEventParameterValue<byte[]>("setCid");

      // let's do some crosscheck
      if (userAddress != _account.ChecksumAddress())
        throw new vBaseException("The user address in the event does not match the account address");

      if (!newSetCid.SequenceEqual(setCid))
        throw new vBaseException("The set CID in the event does not match the requested set CID");
    }
  }

  /// <summary>
  /// Calls the specified contract function.
  /// </summary>
  /// <param name="functionName">The name of the function to call.</param>
  /// <param name="functionInput">The input parameters for the function.</param>
  /// <returns>The result of the contract function execution.</returns>
  /// <exception cref="vBaseException">Thrown when there is an issue with the contract function execution.</exception>
  private async Task<ContractMethodExecuteResultDto> CallContractFunction(string functionName, params object[] functionInput)
  {
    var function = _commitmentServiceContract.GetFunction(functionName);
    var functionData = function.GetData(functionInput);
    var receipt = await _communicationChannel.CallContractFunction(function, functionData);

    if (!receipt.Success)
      throw new vBaseException($"Failed to call contract function {functionName}");

    return receipt.Data;
  }

  private async Task<TResultType> CallStateVariable<TResultType>(string stateVariableName, params object[] functionInput)
  {
    var function = _commitmentServiceContract.GetFunction(stateVariableName);
    var functionData = function.GetData(functionInput);
    var receipt = await _communicationChannel.CallStateVariable<TResultType>(functionData);

    if (!receipt.Success)
      throw new vBaseException($"Failed to call state variable {stateVariableName}");

    return receipt.Data;
  }
}