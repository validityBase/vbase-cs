using System.Collections.Generic;
using System.Threading.Tasks;
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

    public async Task<Dictionary<string, string>> AddSetObject(byte[] cid, byte[] recordCid)
    {
     return await _commitmentService.AddSetObject(cid, recordCid);
    }
  }
}
