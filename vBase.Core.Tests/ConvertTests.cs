using System.Numerics;
using FluentAssertions;
using Nethereum.Hex.HexConvertors.Extensions;
using vBase.Core.Utilities;
using Convert = vBase.Core.Utilities.Convert;

namespace vBase.Core.Tests;

public class ConvertTests
{
  [TestCase(
    "0x0000000000000000000000000000000000000000000000000000000000000001",
    "0x0000000000000000000000000000000000000000000000000000000000000002",
    "0x0000000000000000000000000000000000000000000000000000000000000003",
    TestName = "Int 256 Addition 1 + 2.")]

  [TestCase(
    "0x0000000000000000000000000000000000000000000000000000000000000000",
    "0x85f62bc4cb22ef8d59817dafc9fbf13222f18a114834437524478eb105f1d6aa",
    "0x85f62bc4cb22ef8d59817dafc9fbf13222f18a114834437524478eb105f1d6aa",
    TestName = "Int 256 Addition. 0 + BigNumber")]

  public void AddInt256Test(string aHex, string bHex, string expectedResHx)
  {
    var a = Convert.EthereumBytesToBigInt(aHex.HexToByteArray());
    var b = Convert.EthereumBytesToBigInt(bHex.HexToByteArray());
    var maxSum = BigInteger.Pow(2, 256);
    var res = ((a + b) % maxSum).BigIntToEthereumBytes(256).ToHex(true);
    res.Should().Be(expectedResHx);
  }
}