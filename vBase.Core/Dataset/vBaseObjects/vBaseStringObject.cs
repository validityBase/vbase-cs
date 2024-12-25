using System;
using Newtonsoft.Json.Linq;
using vBase.Core.Utilities;

namespace vBase.Core.Dataset.vBaseObjects;

public class vBaseStringObject: vBaseObject
{
  /// <summary>
  /// vBase string object name
  /// for bidirectional compatibility with vBase Python SDK the V letter is capitalized
  /// </summary>
  public const string vBaseObjectType = "VBaseStringObject";

  public vBaseStringObject(object data)
    : base(data)
  {
    if (data != null && data.GetType() != typeof(string))
    {
      throw new InvalidOperationException($"{nameof(vBaseStringObject)} can be created only from string.");
    }
  }

  public vBaseStringObject()
  {

  }

  public override JValue? GetJson()
  {
    if (Data == null)
    {
      return null;
    }
    return new JValue(Data);
  }

  public override Cid GetCid()
  {
    return StringData?.GetCid() ?? Cid.Empty;
  }

  public override void InitFromJson(JValue? jData)
  {
    Data = jData?.Value;
  }
}