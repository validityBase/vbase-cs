using System.Text;
using Nethereum.ABI;
using Newtonsoft.Json.Linq;

namespace vBase.Core.Utilities;

public static class CryptoUtils
{
  private static readonly ABIEncode AbiEncode = new();

  public static byte[] GetCid(this object value)
  {
    return AbiEncode.GetSha3ABIEncoded(value);
  }
}