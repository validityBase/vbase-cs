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

    public async Task AddSetObject(string datasetName, object record)
    {
      await _commitmentService.AddSetObject(datasetName, record);
    }

    public async Task<bool> UserNamedSetExists(string datasetName)
    {
      return await _commitmentService.UserSetExists(datasetName);
    }

    public async Task AddNamedSet(string datasetName)
    {
      await _commitmentService.AddSet(datasetName);
    }
  }
}
