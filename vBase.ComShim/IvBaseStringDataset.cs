using System.Runtime.InteropServices;

namespace vBase
{
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [Guid(ComGuids.vBaseStringDatasetInterface)]
  public interface IvBaseStringDataset
  {
    void AddRecord(string recordData);
    IVerificationResult VerifyCommitments();
  }
}