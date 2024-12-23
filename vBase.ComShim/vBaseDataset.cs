using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using vBase.Core.Utilities;

namespace vBase
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid(ComGuids.vBaseDataset)]
  public class vBaseDataset : IvBaseDataset
  {
    private readonly Core.Dataset.vBaseDataset _coreDataset;

    internal vBaseDataset(IvBaseClient client, string name, vBaseDatasetRecordTypes recordType)
    {
      _coreDataset = new Core.Dataset.vBaseDataset(
        ((vBaseClient)client).GetCoreClient(),
        name,
        RecordTypeToCoreRecordType(recordType));
    }

    internal vBaseDataset(IvBaseClient client, string json)
    {
      _coreDataset = new Core.Dataset.vBaseDataset(
        ((vBaseClient)client).GetCoreClient(),
        json);
    }

    public void AddRecord(object recordData)
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

    public string ToJson()
    {
      return _coreDataset.ToJson();
    }

    private string RecordTypeToCoreRecordType(vBaseDatasetRecordTypes vBaseRecordType)
    {
      switch (vBaseRecordType)
      {
        case vBaseDatasetRecordTypes.vBaseStringObject:
          return Core.Dataset.vBaseRecordTypes.vBaseStringObject;
        default:
          throw new System.ArgumentException("Unknown record type: " + vBaseRecordType);
      }
    }
  }
}
