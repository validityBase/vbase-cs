using System.Runtime.InteropServices;

namespace vBase.Receipts
{
  /// <summary>
  /// Represents a transaction receipt.
  /// </summary>
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [Guid(ComGuids.vBaseReceiptInterface)]
  public interface IReceipt
  {
    /// <summary>
    /// The transaction timestamp in Unix time format (seconds).
    /// </summary>
    long Timestamp { get; set; }
  }
}
