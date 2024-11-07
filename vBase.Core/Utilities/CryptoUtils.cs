using System.Numerics;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;

namespace vBase.Core.Utilities;

public static class CryptoUtils
{
  /// <summary>
  /// Get SHA3 256 hash of the input integer.
  /// </summary>
  /// <param name="value">Integer value.</param>
  /// <param name="size">Size in bits.</param>
  /// <returns>SHA3 256 hash object.</returns>
  public static Cid GetCid(this BigInteger value, uint size = 256)
  {
    var intBytes = value.BigIntToEthereumBytes(size);
    return new Cid(GetSha3Hash(intBytes));
  }

  /// <summary>
  /// Get SHA3 256 hash of the input string.
  /// </summary>
  /// <param name="value">input string</param>
  /// <returns>SHA3 256 hash object</returns>
  public static Cid GetCid(this string value)
  {
    byte[] input = Encoding.UTF8.GetBytes(value);
    return new Cid(GetSha3Hash(input));
  }

  public static byte[] GetSha3Hash(byte[] input)
  {
    Sha3Digest sha3 = new Sha3Digest(256);
    sha3.BlockUpdate(input, 0, input.Length);
    byte[] result = new byte[sha3.GetDigestSize()];
    sha3.DoFinal(result, 0);
    return result;
  }
}