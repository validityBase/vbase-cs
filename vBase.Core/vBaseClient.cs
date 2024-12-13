using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using vBase.Core.Base;
using vBase.Core.Utilities;

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

    public async Task<Dictionary<string, string>> AddSetObject(byte[] cid, byte[] recordCid)
    {
     return await _commitmentService.AddSetObject(cid, recordCid);
    }

    public async Task<bool> UserNamedSetExists(string datasetName)
    {
      return await _commitmentService.UserSetExists(datasetName.GetCid());
    }

    public async Task AddNamedSet(string datasetName)
    {
      await _commitmentService.AddSet(datasetName.GetCid());
    }
  }
}
