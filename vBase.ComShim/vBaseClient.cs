using System;
using System.Runtime.InteropServices;
using vBase.Core;

namespace vBase
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid(ComGuids.vBaseClient)]
  public class vBaseClient: IvBaseClient
  {
    private readonly Core.vBaseClient _coreClient;

    internal vBaseClient(ICommitmentService commitmentService)
    {
      _coreClient = new Core.vBaseClient(commitmentService);
    }

    internal Core.vBaseClient GetCoreClient()
    {
      return _coreClient;
    }

    public long AddSetObject(string datasetName, byte[] objectCid)
    {
      return _coreClient.AddSetObject(datasetName, new Cid(objectCid)).Result.ToUnixTimeSeconds();
    }

    public bool UserNamedSetExists(string owner, string datasetName)
    {
      return _coreClient.UserNamedSetExists(owner, datasetName).Result;
    }

    public void AddNamedSet(string datasetName)
    {
      _coreClient.AddNamedSet(datasetName).Wait();
    }

    public bool VerifyUserObject(string owner, byte[] objectCid, long timestamp)
    {
      return _coreClient.VerifyUserObject(owner, new Cid(objectCid), DateTimeOffset.FromUnixTimeSeconds(timestamp))
        .Result;
    }
  }
}