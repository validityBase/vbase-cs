using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADRaffy.ENSNormalize;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json.Linq;
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
    var contractDefinitionJson = Utilities.Utils.LoadEmbeddedJson("CommitmentService.json");
    _commitmentServiceContract = _web3.Eth.GetContract(contractDefinitionJson, "0x1234");
  }

  public async Task<Dictionary<string, string>> AddSetObject(byte[] setCid, byte[] recordCid)
  {
    var contractMethodExecutionRes = await CallContractFunction("addSetObject", setCid, recordCid);
    return new Dictionary<string, string>();
  }

  public async Task<bool> UserSetExists(byte[] setCid)
  {
    var contractMethodExecutionRes = await CallContractFunction("userSetCommitments", _account.ChecksumAddress(), setCid);
    return contractMethodExecutionRes.Status.StartsWith("1");
  }

  /// <summary>
  /// Creates a new set with the specified setCid.
  /// If the set already exists, no action will be taken.
  /// </summary>
  /// <param name="setCid">The CID representing the name of the new set.</param>
  public async Task AddSet(byte[] setCid)
  {
    var contractMethodExecutionRes = await CallContractFunction("addSet", setCid);
    var addSetEvents = _commitmentServiceContract.GetEvent("AddSet")
      .DecodeAllEventsDefaultForEvent(contractMethodExecutionRes.Logs);

    var setCreationEvent = addSetEvents.SingleOrDefault();
    if (setCreationEvent != null)
    {
      // a new set has been created
      EventLog<List<ParameterOutput>> a  = setCreationEvent;
      string userAddress = setCreationEvent.GetResult<string>("user");
      byte[] newSetCid = setCreationEvent.GetResult<byte[]>("setCid");

      // let's do some crosscheck
      if (userAddress != _account.ChecksumAddress())
      {
        throw new vBaseException("The user address in the event does not match the account address");
      }

      if (!newSetCid.SequenceEqual(setCid))
      {
        throw new vBaseException("The set CID in the event does not match the requested set CID");
      }
    }
  }

  private async Task<ContractMethodExecuteResultDto> CallContractFunction(string functionName, params object[] functionInput)
  {
    var function = _commitmentServiceContract.GetFunction(functionName);
    var functionData = function.GetData(functionInput);
    var receipt = await _communicationChannel.CallContractFunction(function, functionData);
    if (!receipt.Success)
    {
      throw new vBaseException($"Failed to call contract function {functionName}");
    }
    return receipt.Data;
  }

  
}