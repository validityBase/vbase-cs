using System.Runtime.InteropServices;
using Nethereum.Hex.HexConvertors.Extensions;

namespace vBase.Core
{
  public class Cid
  {
    public byte[] Data { get; private set; }

    public Cid(byte[] data)
    {
      Data = data;
    }

    public Cid(string data)
    {
      Data = data.HexToByteArray();
    }

    public string ToHex()
    {
      return Data.ToHex(true);
    }

    public static Cid Empty { get; private set; } = new Cid(new byte[32]);
  }
}
