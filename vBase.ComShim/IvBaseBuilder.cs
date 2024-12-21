using System.Runtime.InteropServices;

namespace vBase
{
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [Guid(ComGuids.vBaseBuilderInterface)]
  public interface IvBaseBuilder
  {
    IvBaseClient CreateForwarderClient(string forwarderUrl, string apiKey, string privateKey);
    IvBaseStringDataset CreateStringDataset(IvBaseClient client, string name);
  }
}