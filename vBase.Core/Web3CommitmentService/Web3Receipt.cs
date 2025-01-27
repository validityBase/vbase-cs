using System;

namespace vBase.Core.Web3CommitmentService;

/// <summary>
/// WEB3-specific receipt.
/// Additionally to the base timestamp, it contains the transaction hash.
/// </summary>
public class Web3Receipt(string vBaseTransactionHash, DateTimeOffset vBaseTimestamp)
  : Receipt(vBaseTimestamp)
{
  public string TransactionHash { get; set; } = vBaseTransactionHash;
}