using System.Collections.Generic;
using System.Threading.Tasks;
using ADRaffy.ENSNormalize;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using vBase.Core.DTOs;
using vBase.Core.Exceptions;

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
    var contractMethodExecuteRes = await CallContractFunction("addSetObject", setCid, recordCid);
    return new Dictionary<string, string>();
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