using System;
using System.Runtime.InteropServices;
using vBase.Receipts;

namespace vBase.Infrastructure
{
  /// <summary>
  /// Converts between <see cref="Core.Receipt"/> and COM-compatible <see cref="IReceipt"/>.
  /// </summary>
  [ComVisible(false)]
  internal static class ReceiptConverter
  {
    /// <summary>
    /// Converts a <see cref="vBase.Core.Receipt"/> to a <see cref="vBase.Receipts.IReceipt"/>.
    /// </summary>
    /// <param name="receipt">The receipt to convert.</param>
    /// <returns>The converted receipt.</returns>
    public static IReceipt ToCom(Core.Receipt receipt)
    {
      if (receipt is Core.Web3CommitmentService.Web3Receipt web3Receipt)
      {
        return new Web3Receipt()
        {
          Timestamp = web3Receipt.Timestamp.ToUnixTimeSeconds(),
          TransactionHash = web3Receipt.TransactionHash
        };
      }

      throw new NotSupportedException($"Unsupported receipt type: {receipt.GetType().Name}");
    }
  }
}
