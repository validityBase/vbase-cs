using System.Collections.Generic;
using System.Threading.Tasks;

namespace vBase.Core.Base;

public class CommitmentService(ICommunicationChannel CommunicationChannel)
{
  public async Task<Dictionary<string, string>> AddSetObject(string setCid, string recordCid)
  {
    return await CommunicationChannel.CallMethod("addSetObject", setCid, recordCid);
  }
}