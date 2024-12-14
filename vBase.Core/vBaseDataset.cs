using System;
using System.Threading.Tasks;
using vBase.Core.Utilities;

namespace vBase.Core;

public class vBaseDataset<TDataType>
{
  private readonly vBaseClient _vBaseClient;
  private readonly string _name;

  public vBaseDataset(vBaseClient vBaseClient, string name)
  {
    _vBaseClient = vBaseClient;
    _name = name;

    Initialize().Wait();
  }

  public async Task Initialize()
  {
    if (!await _vBaseClient.UserNamedSetExists(_name))
    {
      await _vBaseClient.AddNamedSet(_name);
    }
  }

  public async Task AddRecord(TDataType recordData)
  {
    if(recordData == null)
    {
      throw new ArgumentNullException(nameof(recordData));
    }

    var recordCid = recordData.GetCid();
    await _vBaseClient.AddSetObject(_name, recordCid);
  }
}