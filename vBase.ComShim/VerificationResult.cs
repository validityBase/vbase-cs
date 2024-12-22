using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace vBase
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid(ComGuids.VerificationResult)]
  public class VerificationResult: IVerificationResult
  {
    private readonly List<string> _verificationFindings = new List<string>();

    public bool VerificationPassed => _verificationFindings.Count == 0;
    public string[] VerificationFindings => _verificationFindings.ToArray();

    internal void AddFinding(string finding)
    {
      _verificationFindings.Add(finding);
    }
  }
}
