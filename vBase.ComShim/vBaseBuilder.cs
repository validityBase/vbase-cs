using System.Runtime.InteropServices;
using vBase.Core;
using vBase.Core.Web3CommitmentService;
using vBase.Infrastructure;

namespace vBase
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid(ComGuids.vBaseBuilder)]
 public class vBaseBuilder: IvBaseBuilder
  {
    static vBaseBuilder()
    {
      AssemblyResolver.Register();
    }

    public IvBaseClient CreateForwarderClient(string forwarderUrl, string apiKey, string privateKey)
    {
      ICommitmentService commitmentService = new ForwarderCommitmentService(forwarderUrl, apiKey, privateKey);
      return new vBaseClient(commitmentService);
    }

    public IvBaseDataset CreateDataset(IvBaseClient client, string name, vBaseDatasetRecordTypes recordType)
    {
      return new vBaseDataset(client, name, recordType);
    }

    public IvBaseDataset CreateDatasetFromJson(IvBaseClient client, string json)
    {
      return new vBaseDataset(client, json);
    }
  }
}