using Newtonsoft.Json.Linq;

namespace vBase.Core.Dataset.vBaseObjects;

/// <summary>
/// Base class for all vBase objects.
/// Each implementation should provide a constructor with one object parameter, and parameterless constructor.
/// </summary>
public abstract class vBaseObject
{
  public vBaseObject(object data)
  {
    Data = data;
  }

  public vBaseObject() { }

  public object? Data { get; protected set; }

  public string? StringData => (string?)Data;

  public abstract void InitFromJson(JValue? jData);

  public abstract JValue? GetJson();

  public abstract Cid GetCid();
}