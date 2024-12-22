using System.Threading.Tasks;
using Nethereum.Contracts;
using vBase.Core.DTOs;

namespace vBase.Core.Base;

/// <summary>
/// Allows to communicate with a smart Contract deployed to Ethereum blockchain.
/// </summary>
public interface ICommunicationChannel
{
  /// <summary>
  /// Executes Smart Contract function.
  /// </summary>
  /// <param name="function">Function descriptor.</param>
  /// <param name="functionData">Data which will be passed as a function arguments.</param>
  /// <returns></returns>
  Task<ReceiptDto<ContractMethodExecuteResultDto>> CallContractFunction(Function function, string functionData);

  /// <summary>
  /// Fetches state variable from the Smart Contract.
  /// </summary>
  /// <typeparam name="TResultType">Expected result type</typeparam>
  /// <param name="functionData">Encoded state variable</param>
  /// <returns>Variable value</returns>
  Task<ReceiptDto<TResultType>> FetchStateVariable<TResultType>(string functionData);
}