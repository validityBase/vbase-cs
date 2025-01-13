using System.Net;

namespace vBase.Infrastructure
{
  public static class SecurityProtocolHelper
  {
    /// <summary>
    /// When running the shim in the VBA environment, we observed on some machines
    /// that the security protocol is explicitly set to Ssl3 or Tls.
    /// Such a configuration is incompatible with TLS 1.2, which is the protocol used by the Forwarder server.
    /// Microsoft recommends using SystemDefault and avoiding explicitly specifying protocols.
    /// For more details, see: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/tls
    /// </summary>
    public static void ResetSecurityProtocol()
    {
      ServicePointManager.SecurityProtocol = 0; // set to SystemDefault
    }
  }
}
