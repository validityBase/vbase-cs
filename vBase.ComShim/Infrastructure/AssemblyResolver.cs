using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using vBase.Core.Utilities;

namespace vBase.Infrastructure
{
  /// <summary>
  /// Some dependencies are referenced transitively multiple times with different versions.
  /// Only the latest version is available in the application folder.
  /// To resolve older versions at runtime and return the latest version,
  /// we use the AssemblyResolver class.
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

      string directory = Path.GetDirectoryName(typeof(AssemblyResolver).Assembly.Location);
      string assemblyFileName = $"{assemblyName.Name}.dll";
      string assemblyFilePath = Path.Combine(directory.AsserNotNull(), assemblyFileName);
      return Assembly.LoadFrom(assemblyFilePath);
    }
  }
}
