using System.Runtime.InteropServices;
using vBase.Core.Base;
using vBase.Core.CommunicationChannels;
using vBase.Infrastructure;

namespace vBase
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid(ComGuids.vBaseBuilder)]
 public class vBaseBuilder: IvBaseBuilder
  {
    static vBaseBuilder()
    {
      AssemblyResolver.Register();
    }

    public IvBaseClient CreateForwarderClient(string forwarderUrl, string apiKey, string privateKey)
    {
      ICommunicationChannel channel = new ForwarderCommunicationChannel(forwarderUrl, apiKey, privateKey);
      return new vBaseClient(channel, privateKey);
    }

    public IvBaseStringDataset CreateStringDataset(IvBaseClient client, string name)
    {
      return new vBaseStringDataset(client, name);
    }
  }
}