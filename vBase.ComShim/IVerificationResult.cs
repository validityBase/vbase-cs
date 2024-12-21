using System.Runtime.InteropServices;

namespace vBase
{
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [Guid(ComGuids.VerificationResultInterface)]
  public interface IVerificationResult
  {
    bool VerificationPassed { get; }
    string[] VerificationFindings { get; }
  }
}