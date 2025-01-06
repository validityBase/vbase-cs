using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using vBase.Core;

namespace vBase
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid(ComGuids.vBaseClient)]
  public class vBaseClient: IvBaseClient
  {
    private readonly Core.vBaseClient _coreClient;
    private readonly ILogger _logger;

    internal vBaseClient(ICommitmentService commitmentService, ILogger logger)
    {
      _logger = logger;
      _coreClient = new Core.vBaseClient(commitmentService);
    }

    internal Core.vBaseClient GetCoreClient()
    {
      return _coreClient;
    }

    public long AddSetObject(string setCid, string objectCid)
    {
      return Utils.PreprocessException(() =>
        _coreClient.AddSetObject(new Cid(setCid), new Cid(objectCid)).Result.ToUnixTimeSeconds(), _logger);
    }

    public bool UserNamedSetExists(string user, string name)
    {
      return Utils.PreprocessException(() =>
        _coreClient.UserNamedSetExists(user, name).Result, _logger);
    }

    public void AddNamedSet(string name)
    {
      Utils.PreprocessException(() => _coreClient.AddNamedSet(name).Wait(), _logger);
    }

    public void AddSet(string setCid)
    {
      Utils.PreprocessException(() => _coreClient.AddSet(new Cid(setCid)).Wait(), _logger);
    }

    public bool VerifyUserObject(string user, string objectCid, long timestamp)
    {
      return Utils.PreprocessException(() => 
        _coreClient.VerifyUserObject(user, new Cid(objectCid), DateTimeOffset.FromUnixTimeSeconds(timestamp))
        .Result, _logger);
    }

    public bool VerifyUserSetObjects(string user, string setCid, string userSetObjectsCidSum)
    {
      return Utils.PreprocessException(() => 
        _coreClient.VerifyUserSetObjects(user, new Cid(setCid), BigInteger.Parse(userSetObjectsCidSum))
          .Result, _logger);
    }
  }
}