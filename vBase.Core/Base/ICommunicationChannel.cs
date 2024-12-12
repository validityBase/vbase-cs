using System.Collections.Generic;
using System.Threading.Tasks;
using Nethereum.Contracts;
using vBase.Core.DTOs;

namespace vBase.Core.Base;

public interface ICommunicationChannel
{
  Task<ReceiptDto<ContractMethodExecuteResultDto>> CallContractFunction(Function function, string functionData);
}