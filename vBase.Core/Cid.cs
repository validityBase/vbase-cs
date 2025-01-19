using System.Runtime.InteropServices;
using Nethereum.Hex.HexConvertors.Extensions;

namespace vBase.Core
{
  /// <summary>
  /// Content Identifier (CID) is used to uniquely identify objects.
  /// </summary>
  public class Cid
  {
    /// <summary>
    /// The data of the CID.
    /// </summary>
    public byte[] Data { get; private set; }

    /// <summary>
    /// Creates a new CID from the provided byte array.
    /// </summary>
    public Cid(byte[] data)
    {
      Data = data;
    }

    /// <summary>
    /// Creates a new CID from the provided hex string.
    /// </summary>
    /// <param name="data"></param>
    public Cid(string data)
    {
      Data = data.HexToByteArray();
    }

    /// <summary>
    /// Returns the CID as a hex string.
    /// </summary>
    /// <returns>Hex string.</returns>
    public string ToHex()
    {
      return Data.ToHex(true);
    }

    /// <summary>
    /// Empty CID.
    /// </summary>
    public static Cid Empty { get; private set; } = new Cid(new byte[32]);
  }
}
