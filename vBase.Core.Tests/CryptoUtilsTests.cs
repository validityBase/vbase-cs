using System.Text;
using FluentAssertions;
using Nethereum.Hex.HexConvertors.Extensions;
using vBase.Core.Utilities;

namespace vBase.Core.Tests;

public class CryptoUtilsTests
{
  [TestCase("TestRecord", "0x5cbda6a837e3c6aebd2ce2f14510d4234b870d415ac06f4e411d995504232d2d")]
  [TestCase(1, "0xb79151ec5d30a80b78789805f293fa4fb8fd1eebc0c9367e7c9106678a893df1")]
  [TestCase(1968374, "0xbef23e694ee8fa4edce2a021c31b070ebbde7f36942fc3c7999bd01509bf3f69")]
  public void GetCid_CompatibilityWithPy_vBaseSdk_Test(object data, string cid)
  {
    data.GetCid().ToHex(true).Should().Be(cid);
  }
}