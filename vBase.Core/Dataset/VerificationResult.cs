using System.Collections.Generic;

namespace vBase.Core.Dataset
{
  public class VerificationResult
  {
    private readonly List<string> _verificationFindings = new();

    public bool VerificationPassed => _verificationFindings.Count == 0;
    public string[] VerificationFindings => _verificationFindings.ToArray();

    public void AddFinding(string finding)
    {
      _verificationFindings.Add(finding);
    }
  }
}
