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

  /// <summary>
  /// The data stored in the object.
  /// </summary>
  public object? Data { get; protected set; }

  /// <summary>
  /// String representation of the data.
  /// </summary>
  public string? StringData => (string?)Data;

  /// <summary>
  /// Initializes the object from a JSON object.
  /// </summary>
  /// <param name="jData">Json value.</param>
  public abstract void InitFromJson(JValue? jData);


  /// <summary>
  /// Serializes the object to a JSON value.
  /// </summary>
  /// <returns></returns>
  public abstract JValue? GetJson();

  /// <summary>
  /// Returns the <see cref="Cid"/> of the object.
  /// </summary>
  /// <returns>CID (Content Identifiers) for the current object</returns>
  public abstract Cid GetCid();
}