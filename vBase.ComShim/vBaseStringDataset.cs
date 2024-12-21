using System.Runtime.InteropServices;
using vBase.Core.Dataset;

namespace vBase
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid(ComGuids.vBaseStringDataset)]
  public class vBaseStringDataset : IvBaseStringDataset
  {
    private readonly vBaseDataset<string> _coreDataset;

    internal vBaseStringDataset(IvBaseClient client, string name)
    {
      _coreDataset = new vBaseDataset<string>(
        ((vBaseClient)client).GetCoreClient(),
        name);
    }

    public void AddRecord(string recordData)
    {
      _coreDataset.AddRecord(recordData).Wait();
    }

    public IVerificationResult VerifyCommitments()
    {
      var coreVerificationResult = _coreDataset.VerifyCommitments().Result;
      var verificationResult = new VerificationResult();
      foreach (var finding in coreVerificationResult.VerificationFindings)
      {
        verificationResult.AddFinding(finding);
      }
      return verificationResult;
    }
  }
}
