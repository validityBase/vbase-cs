using System;
using System.Numerics;
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

    public long AddSetObject(string setCid, string objectCid)
    {
      return Utils.PreprocessException(() =>
        _coreClient.AddSetObject(new Cid(setCid), new Cid(objectCid)).Result.ToUnixTimeSeconds());
    }

    public bool UserNamedSetExists(string user, string name)
    {
      return Utils.PreprocessException(() =>
        _coreClient.UserNamedSetExists(user, name).Result);
    }

    public void AddNamedSet(string name)
    {
      Utils.PreprocessException(() => _coreClient.AddNamedSet(name).Wait());
    }

    public void AddSet(string setCid)
    {
      Utils.PreprocessException(() => _coreClient.AddSet(new Cid(setCid)).Wait());
    }

    public bool VerifyUserObject(string user, string objectCid, long timestamp)
    {
      return Utils.PreprocessException(() => 
        _coreClient.VerifyUserObject(user, new Cid(objectCid), DateTimeOffset.FromUnixTimeSeconds(timestamp))
        .Result);
    }

    public bool VerifyUserSetObjects(string user, string setCid, string userSetObjectsCidSum)
    {
      return Utils.PreprocessException(() => 
        _coreClient.VerifyUserSetObjects(user, new Cid(setCid), BigInteger.Parse(userSetObjectsCidSum)).Result);
    }
  }
}