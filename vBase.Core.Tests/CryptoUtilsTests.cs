using System.Numerics;
using System.Text;
using FluentAssertions;
using Nethereum.Hex.HexConvertors.Extensions;
using vBase.Core.Utilities;

namespace vBase.Core.Tests;

public class CryptoUtilsTests
{
  [TestCase("TestRecord", "0x5cbda6a837e3c6aebd2ce2f14510d4234b870d415ac06f4e411d995504232d2d",
    TestName = "String Hashing. Py vBase SDK Compatibility Check.")]
  public void GetObjectCid_CompatibilityWithPy_vBaseSdk_Test(object data, string cid)
  {
    data.GetCid().ToHex(true).Should().Be(cid);
  }


  [TestCase(1968374, "0xbef23e694ee8fa4edce2a021c31b070ebbde7f36942fc3c7999bd01509bf3f69", 256u,
    TestName = "Int 256 Hashing. Py vBase SDK Compatibility Check.")]
  [TestCase(1968374, "0x85f62bc4cb22ef8d59817dafc9fbf13222f18a114834437524478eb105f1d6aa", 64u,
    TestName = "Int 64 Hashing. Py vBase SDK Compatibility Check.")]
  public void GetIntCid_CompatibilityWithPy_vBaseSdk_Test(int data, string cid, uint size)
  {
    BigInteger.Parse(data.ToString())
      .GetCid(size)
      .ToHex(true)
      .Should()
      .Be(cid);
  }


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
    var a = CryptoUtils.EthereumBytesToBigInt(aHex.HexToByteArray());
    var b = CryptoUtils.EthereumBytesToBigInt(bHex.HexToByteArray());
    var res = a.Add(b).BigIntToEthereumBytes(256).ToHex(true);
    res.Should().Be(expectedResHx);
  }
}