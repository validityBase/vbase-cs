using System.Collections.Generic;
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
  }

  public string Cid => CryptoUtils.HashTypedValues(_name);

  public async Task<Dictionary<string, string>> AddRecord(TDataType recordData)
  {
    var recordCid = recordData.GetCid();
    var commitmentLog = await _vBaseClient.AddSetObject(Cid, recordCid);
    //_add_record_worker
    return commitmentLog;
  }
}