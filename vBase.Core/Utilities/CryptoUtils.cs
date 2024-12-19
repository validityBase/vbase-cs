using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Org.BouncyCastle.Crypto.Digests;

namespace vBase.Core.Utilities;

public static class CryptoUtils
{
  /// <summary>
  /// Get SHA3 256 hash of the input data.
  /// </summary>
  /// <param name="value">input data</param>
  /// <returns>SHA3 256 hash bytes</returns>
  public static byte[] GetCid(this object value)
  {
    byte[] input = GetBytesFromData(value);
    return GetSha3Hash(input);
  }

  /// <summary>
  /// Get SHA3 256 hash of the input integer.
  /// </summary>
  /// <param name="value">Integer value.</param>
  /// <param name="size">Size in bits.</param>
  /// <returns>SHA3 256 hash bytes</returns>
  public static byte[] GetCid(this BigInteger value, uint size = 256)
  {
    if (size % 8 != 0)
    {
      throw new ArgumentException("Size must be a multiple of 8.");
    }

    var sizeInBytes = size / 8;

    byte[] intBytes = value.ToByteArray().Reverse().ToArray();
    if (intBytes.Length > sizeInBytes)
    {
      throw new ArgumentException($"Integer value {value} is too large for {size} bits.");
    }
    byte[] intResult = new byte[sizeInBytes];
    intBytes.CopyTo(intResult, sizeInBytes - intBytes.Length);
    return GetSha3Hash(intResult);
  }

  public static string ChecksumAddress(this Account account)
  {
    return AddressUtil.Current.ConvertToChecksumAddress(account.Address);
  }

  private static byte[] GetSha3Hash(byte[] input)
  {
    Sha3Digest sha3 = new Sha3Digest(256);
    sha3.BlockUpdate(input, 0, input.Length);
    byte[] result = new byte[sha3.GetDigestSize()];
    sha3.DoFinal(result, 0);
    return result;
  }

  private static byte[] GetBytesFromData(object data)
  {
    if (data is string dataStr)
    {
      return Encoding.UTF8.GetBytes(dataStr);
    }

    throw new NotSupportedException($"Data type {data.GetType()} is not supported.");
  }
}