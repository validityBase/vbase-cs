using System.Runtime.InteropServices;

namespace vBase
{
  /// <summary>
  /// Represents the result of a verification operation.
  /// </summary>
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [Guid(ComGuids.VerificationResultInterface)]
  public interface IVerificationResult
  {
    /// <summary>
    /// Indicates whether the verification passed.
    /// </summary>
    bool VerificationPassed { get; }

    /// <summary>
    /// A collection of verification findings.
    /// </summary>
    string[] VerificationFindings { get; }
  }
}