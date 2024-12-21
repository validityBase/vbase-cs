using System.Runtime.InteropServices;

namespace vBase
{
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [Guid(ComGuids.vBaseClientInterface)]
  public interface IvBaseClient
  {
    long AddSetObject(string datasetName, object record);
    bool UserNamedSetExists(string owner, string datasetName);
    void AddNamedSet(string datasetName);
    bool VerifyUserObject(string owner, byte[] objectCid, long timestamp);
  }
}