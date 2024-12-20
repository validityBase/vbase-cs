using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Nethereum.RLP;
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
    var intBytes = value.BigIntToEthereumBytes(size);
    return GetSha3Hash(intBytes);
  }

  public static BigInteger Normalize256(this BigInteger number)
  {
    return number % BigInteger.Pow(2, 256);
  }

  public static BigInteger Add(this BigInteger a, BigInteger b)
  {
    return (a + b).Normalize256();
  }

  public static string ChecksumAddress(this Account account)
  {
    return AddressUtil.Current.ConvertToChecksumAddress(account.Address);
  }

  public static byte[] BigIntToEthereumBytes(this BigInteger value, uint size)
  {
    if (size % 8 != 0)
    {
      throw new ArgumentException("Size must be a multiple of 8.");
    }

    if (value < 0)
    {
      throw new ArgumentException("Negative values are not supported.");
    }

    var sizeInBytes = size / 8;

    byte[] intBytes = value.ToByteArray().Reverse().ToArray().TrimZeroBytes();
    if (intBytes.Length > sizeInBytes)
    {
      throw new ArgumentException($"Integer value {value} is too large for {size} bits.");
    }

    byte[] intResult = new byte[sizeInBytes];
    intBytes.CopyTo(intResult, sizeInBytes - intBytes.Length);

    return intResult;
  }

  public static BigInteger EthereumBytesToBigInt(byte[] bytes)
  {
    var bigIntBytes = bytes.SkipWhile(b => b == 0).Reverse().ToList();
    bigIntBytes.Add(0); // add sign byte
    var bigInt = new BigInteger(bigIntBytes.ToArray());
    return  bigInt;
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