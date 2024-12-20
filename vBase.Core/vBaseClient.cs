using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3.Accounts;
using vBase.Core.Base;

namespace vBase.Core
{
  /// <summary>
  /// Provides Python validityBase (vBase) access.
  /// </summary>
  public class vBaseClient
  {
    private readonly CommitmentService _commitmentService;

    public vBaseClient(CommitmentService commitmentService)
    {
      _commitmentService = commitmentService;
    }

    public Account Account => _commitmentService.Account;

    public async Task<DateTimeOffset> AddSetObject(string datasetName, object record)
    {
      return await _commitmentService.AddSetObject(datasetName, record);
    }

    public async Task<bool> UserNamedSetExists(string owner, string datasetName)
    {
      return await _commitmentService.UserSetExists(owner, datasetName);
    }

    public async Task AddNamedSet(string datasetName)
    {
      await _commitmentService.AddSet(datasetName);
    }

    public async Task<bool> VerifyUserObject(string owner, byte[] objectCid, DateTimeOffset timestamp)
    {
      return await _commitmentService.VerifyUserObject(owner, objectCid, timestamp);
    }

    public async Task<bool> VerifyUserSetObjects(string owner, byte[] objectCid, BigInteger setObjectCidSum)
    {
      return await _commitmentService.VerifyUserSetObjects(owner, objectCid, setObjectCidSum);
    }
  }
}
