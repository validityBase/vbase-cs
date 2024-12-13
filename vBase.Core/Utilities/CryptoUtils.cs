using System.Text;
using Nethereum.ABI;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json.Linq;

namespace vBase.Core.Utilities;

public static class CryptoUtils
{
  private static readonly ABIEncode AbiEncode = new();

  public static byte[] GetCid(this object value)
  {
    return AbiEncode.GetSha3ABIEncoded(value);
  }

  public static string ChecksumAddress(this Account account)
  {
    return Nethereum.Util.AddressUtil.Current.ConvertToChecksumAddress(account.Address);
  }
}