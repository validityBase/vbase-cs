using System.Net;
using System.Runtime.InteropServices;

namespace vBase.Infrastructure
{
  /// <summary>
  /// When running the shim in the VBA environment, we observed on some machines
  /// that the security protocol is explicitly set to Ssl3 or Tls.
  /// Such a configuration is incompatible with TLS 1.2, which is the protocol used by the Forwarder server.
  /// Experimentally, we found that setting the security protocol to 0 (SystemDefault) does not resolve the issue.
  /// Setting explicitly to Tls12 does.
  /// </summary>
  [ComVisible(false)]
  internal static class SecurityProtocolHelper
  {
    /// <summary>
    /// Updates the security protocol to Tls12.
    /// </summary>
    public static void ResetSecurityProtocol()
    {
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    }
  }
}
