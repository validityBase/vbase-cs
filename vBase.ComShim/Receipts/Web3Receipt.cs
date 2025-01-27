using System.Runtime.InteropServices;

namespace vBase.Receipts
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid(ComGuids.vBaseWeb3Receipt)]
  public class Web3Receipt : IWeb3Receipt
  {
    public long Timestamp { get; set; }

    public string TransactionHash { get; set; }
  }
}