using System;
using System.Linq;
using System.Numerics;
using Nethereum.RLP;
namespace vBase.Core.Utilities
{
  /// <summary>
  /// Provides conversion methods.
  /// </summary>
  public static class Convert
  {
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
      return bigInt;
    }

    public static BigInteger CidToBigInt(this Cid cid)
    {
      var bigIntBytes = cid.Data.SkipWhile(b => b == 0).Reverse().ToList();
      bigIntBytes.Add(0); // add sign byte
      var bigInt = new BigInteger(bigIntBytes.ToArray());
      return bigInt;
    }
  }
}
