using System;
using System.Collections.Generic;

namespace vBase.Core.Dataset.vBaseObjects;

internal static class vBaseObjectFactory {

  private static readonly Dictionary<string, Type> RegisteredTypes = new();

  static vBaseObjectFactory()
  {
    RegisteredTypes.Add(vBaseStringObject.vBaseObjectType, typeof(vBaseStringObject));
  }

  public static vBaseObject Create(string typeKey, object data)
  {
    var instance = (vBaseObject)Activator.CreateInstance(GetRegisteredType(typeKey), data);
    return instance;
  }

  public static vBaseObject Create(string typeKey)
  {
    var instance = (vBaseObject)Activator.CreateInstance(GetRegisteredType(typeKey));
    return instance;
  }

  public static bool IsTypeRegistered(string vBaseObjectTypeName)
  {
    return RegisteredTypes.ContainsKey(vBaseObjectTypeName);
  }

  private static Type GetRegisteredType(string typeKey)
  {
    if (!RegisteredTypes.ContainsKey(typeKey))
    {
      throw new InvalidOperationException($"vBase object typeKey {typeKey} is not registered.");
    }

    return RegisteredTypes[typeKey];
  }
}