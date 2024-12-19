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


}