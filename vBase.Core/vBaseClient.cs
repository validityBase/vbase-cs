using System;
using System.Numerics;
using System.Threading.Tasks;

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

    public async Task<DateTimeOffset> AddSetObject(string datasetName, Cid objectToAddCid)
    {
      return await _commitmentService.AddSetObject(datasetName, objectToAddCid);
    }

    public async Task<bool> UserNamedSetExists(string owner, string datasetName)
    {
      return await _commitmentService.UserSetExists(owner, datasetName);
    }

    public async Task AddNamedSet(string datasetName)
    {
      await _commitmentService.AddSet(datasetName);
    }

    public async Task<bool> VerifyUserObject(string owner, Cid objectCid, DateTimeOffset timestamp)
    {
      return await _commitmentService.VerifyUserObject(owner, objectCid, timestamp);
    }

    public async Task<bool> VerifyUserSetObjects(string owner, Cid objectCid, BigInteger setObjectsCidSum)
    {
      return await _commitmentService.VerifyUserSetObjects(owner, objectCid, setObjectsCidSum);
    }
  }
}
