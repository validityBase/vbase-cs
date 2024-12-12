using System.Text;
using Nethereum.ABI;

namespace vBase.Core.Utilities;

public static class CryptoUtils
{
    private static readonly ABIEncode AbiEncode = new();
    public static string HashTypedValues(params object[] values)
    {
        var hashBytes = AbiEncode.GetSha3ABIEncoded(values);
        return ByteArrayToString(hashBytes);
    }

    public static string GetCid(this object value)
    {
      return HashTypedValues(value);
    }

    public static string ByteArrayToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

}