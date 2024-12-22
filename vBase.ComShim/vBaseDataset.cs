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
    /// <summary>
    /// In vBase.Core, Dataset is a generic class. However, at the COM interface, we cannot expose generic types.
    /// This class moves the generic type argument to the vBaseDatasetRecordTypes enum value.
    /// To achieve this, we need to use reflection, as the exact type of TDataType is not known at compile time.
    /// </summary>
    private readonly object _coreDataset;

    internal vBaseDataset(IvBaseClient client, string name, vBaseDatasetRecordTypes recordType)
    {
      _coreDataset = typeof(Core.Dataset.vBaseDataset<>)
        .MakeGenericType(GetDatasetRecordType(recordType))
        .GetConstructor(new[] { typeof(Core.vBaseClient), typeof(string) }).AsserNotNull()
        .Invoke(new object[] { ((vBaseClient)client).GetCoreClient(), name });
    }

    public void AddRecord(object recordData)
    {
      Task asyncTask = (Task)_coreDataset
        .GetType()
        .GetMethod("AddRecord")
        .AsserNotNull()
        .Invoke(_coreDataset, new[] { recordData });

      asyncTask.Wait();
    }

    public IVerificationResult VerifyCommitments()
    {
      Task<Core.Dataset.VerificationResult> asyncTask = (Task<Core.Dataset.VerificationResult>)_coreDataset
        .GetType()
        .GetMethod("VerifyCommitments")
        .AsserNotNull()
        .Invoke(_coreDataset, Array.Empty<object>());

      var coreVerificationResult = asyncTask.Result;

      var verificationResult = new VerificationResult();
      foreach (var finding in coreVerificationResult.VerificationFindings)
      {
        verificationResult.AddFinding(finding);
      }
      return verificationResult;
    }

    private Type GetDatasetRecordType(vBaseDatasetRecordTypes recordType)
    {
      switch (recordType)
      {
        case vBaseDatasetRecordTypes.String: return typeof(string);
        default: throw new ArgumentException($"Unsupported record type: {recordType}");
      }
    }
  }
}
