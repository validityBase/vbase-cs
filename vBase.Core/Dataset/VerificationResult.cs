using System.Collections.Generic;

namespace vBase.Core.Dataset
{
  /// <summary>
  /// Contains a list of verification findings.
  /// <see cref="vBaseDataset.VerifyCommitments"/>
  /// </summary>
  public class VerificationResult
  {
    private readonly List<string> _verificationFindings = [];

    /// <summary>
    /// Indicates whether the verification passed.
    /// </summary>
    public bool VerificationPassed => _verificationFindings.Count == 0;

    /// <summary>
    /// A collection of verification findings.
    /// </summary>
    public string[] VerificationFindings => _verificationFindings.ToArray();

    /// <summary>
    /// Adds a finding to the verification result.
    /// </summary>
    /// <param name="finding"></param>
    public void AddFinding(string finding)
    {
      _verificationFindings.Add(finding);
    }
  }
}
