using System.Runtime.InteropServices;

namespace vBase.Receipts
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid(ComGuids.vBaseReceipt)]
  public class Receipt: IReceipt
  {
    public long Timestamp { get; set; }
  }
}