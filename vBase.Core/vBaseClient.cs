using System;
using System.Numerics;
using System.Threading.Tasks;
using vBase.Core.Utilities;

namespace vBase.Core
{
  /// <summary>
  /// Provides Python validityBase (vBase) access.
  /// </summary>
  public class vBaseClient
  {
    private readonly ICommitmentService _commitmentService;

    public vBaseClient(ICommitmentService commitmentService)
    {
      _commitmentService = commitmentService;
    }

    public string AccountIdentifier => _commitmentService.AccountIdentifier;

    public async Task<DateTimeOffset> AddSetObject(Cid setCid, Cid objectCid)
    {
      return await _commitmentService.AddSetObject(setCid, objectCid);
    }

    public async Task<bool> UserNamedSetExists(string user, string name)
    {
      return await _commitmentService.UserSetExists(user, name.GetCid());
    }

    public async Task AddNamedSet(string name)
    {
      await _commitmentService.AddSet(name.GetCid());
    }

    public async Task AddSet(Cid setCid)
    {
      await _commitmentService.AddSet(setCid);
    }

    public async Task<bool> VerifyUserObject(string user, Cid objectCid, DateTimeOffset timestamp)
    {
      return await _commitmentService.VerifyUserObject(user, objectCid, timestamp);
    }

    public async Task<bool> VerifyUserSetObjects(string user, Cid setCid, BigInteger userSetObjectsCidSum)
    {
      return await _commitmentService.VerifyUserSetObjects(user, setCid, userSetObjectsCidSum);
    }
  }
}
