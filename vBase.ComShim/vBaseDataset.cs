using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace vBase
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid(ComGuids.vBaseDataset)]
  public class vBaseDataset : IvBaseDataset
  {
    private readonly Core.Dataset.vBaseDataset _coreDataset;
    private readonly ILogger _logger;

    internal vBaseDataset(IvBaseClient client, string name, ObjectTypes objectType, ILogger logger)
    {
      _logger = logger;
      _coreDataset = new Core.Dataset.vBaseDataset(
        ((vBaseClient)client).GetCoreClient(),
        name,
        ObjectTypeToCoreObjectType(objectType));
    }

    internal vBaseDataset(IvBaseClient client, string json)
    {
      _coreDataset = new Core.Dataset.vBaseDataset(
        ((vBaseClient)client).GetCoreClient(),
        json);
    }

    public void AddRecord(object recordData)
    {
      Utils.PreprocessException(() => _coreDataset.AddRecord(recordData).Wait(), _logger);
    }

    public IVerificationResult VerifyCommitments()
    {
      return Utils.PreprocessException(() =>
      {
        var coreVerificationResult = _coreDataset.VerifyCommitments().Result;

        var verificationResult = new VerificationResult();
        foreach (var finding in coreVerificationResult.VerificationFindings)
        {
          verificationResult.AddFinding(finding);
        }

        return verificationResult;
      }, _logger);
    }

    public string ToJson()
    {
      return Utils.PreprocessException(() => _coreDataset.ToJson(), _logger);
    }

    private string ObjectTypeToCoreObjectType(ObjectTypes vBaseObjectType)
    {
      switch (vBaseObjectType)
      {
        case ObjectTypes.String:
          return Core.Dataset.vBaseObjects.vBaseStringObject.vBaseObjectType;
        default:
          throw new System.ArgumentException("Unknown object type: " + vBaseObjectType);
      }
    }
  }
}
