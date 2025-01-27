using System.Runtime.InteropServices;

namespace vBase.Receipts
{
  /// <summary>
  /// WEB3-specific receipt.
  /// Additionally to the base timestamp, it contains the transaction hash.
  ///
  /// In COM interfaces can inherit from one another.
  /// However, the .NET implementation that exposes the .NET interface to COM
  /// does not support inheritance.
  /// Therefore, you must replicate any interface members in a base interface
  /// to the derived interface.
  /// The interop code does not look at base interface types when building
  /// the exposed COM interface.
  /// </summary>
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [Guid(ComGuids.vBaseWeb3ReceiptInterface)]
  public interface IWeb3Receipt: IReceipt
  {
    /// <summary>
    /// The transaction timestamp in Unix time format (seconds).
    /// </summary>
#pragma warning disable CS0108, CS0114
    long Timestamp { get; set; }
#pragma warning restore CS0108, CS0114

    /// <summary>
    /// The transaction hash.
    /// </summary>
    string TransactionHash { get; set; }
  }
}