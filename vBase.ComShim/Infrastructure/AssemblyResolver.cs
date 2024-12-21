using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using vBase.Core.Utilities;

namespace vBase.Infrastructure
{
  /// <summary>
  /// We have transitive references to two different versions of Newtonsoft.Json - 11.0.0 and 13.0.0.
  /// and in the application folder we have only the latest one
  /// To resolve the older version at runtime and return the latest instead of older version
  /// we use this AssemblyResolver class
  /// </summary>
  [ComVisible(false)]
  public static class AssemblyResolver
  {
    public static void Register()
    {
      AppDomain currentDomain = AppDomain.CurrentDomain;
      currentDomain.AssemblyResolve += OnAssemblyResolve;
    }
    private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
    {
      AssemblyName assemblyName = new AssemblyName(args.Name);
      if (assemblyName.Name.Contains("Newtonsoft.Json"))
      {
        string directory = Path.GetDirectoryName(typeof(AssemblyResolver).Assembly.Location);
        string path = Path.Combine(directory.AsserNotNull(), "Newtonsoft.Json.dll");
        return Assembly.LoadFrom(path);
      }

      return null;
    }
  }
}
