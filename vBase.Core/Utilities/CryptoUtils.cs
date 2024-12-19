using System;
using System.Linq;
using System.Text;
using Nethereum.ABI;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Org.BouncyCastle.Crypto.Digests;

namespace vBase.Core.Utilities;

public static class CryptoUtils
{
  private static readonly ABIEncode AbiEncode = new();

  public static byte[] GetCid(this object value)
  {
    byte[] input = GetBytesFromData(value);
    Sha3Digest sha3 = new Sha3Digest(256);
    sha3.BlockUpdate(input, 0, input.Length);
    byte[] result = new byte[sha3.GetDigestSize()];
    sha3.DoFinal(result, 0);
    return result;
  }

  public static string ChecksumAddress(this Account account)
  {
    return AddressUtil.Current.ConvertToChecksumAddress(account.Address);
  }

  private static byte[] GetBytesFromData(object data)
  {
    if (data is string dataStr)
    {
      return Encoding.UTF8.GetBytes(dataStr);
    }

    if (data is int dataInt)
    {
      var intBytes = BitConverter.GetBytes(dataInt).Reverse().ToArray();
      byte[] intResult = new byte[32];
      intBytes.CopyTo(intResult, 32 - intBytes.Length);
      return intResult;
    }

    throw new NotSupportedException($"Data type {data.GetType()} is not supported.");
  }
}