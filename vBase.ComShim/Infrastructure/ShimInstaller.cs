using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Runtime.InteropServices;

namespace vBase.Infrastructure
{
  /// <summary>
  /// It's important to register the assembly using both versions of regasm—32-bit and 64-bit.
  /// Even though the Excel process is 64-bit, it seems that the VBA execution process is 32-bit,
  /// so it doesn't recognize the registrations in the 64-bit registry.
  /// </summary>
  [RunInstaller(true)]
  [ComVisible(false)]
  public class ShimInstaller : Installer
  {
    protected override void OnAfterInstall(IDictionary savedState)
    {
      Log($"Running {nameof(OnAfterInstall)}");
      base.OnAfterInstall(savedState);
      RunRegasm(Get32BitRuntimeDirectory(), false);
      RunRegasm(Get64BitRuntimeDirectory(), false);
    }

    protected override void OnBeforeUninstall(IDictionary savedState)
    {
      Log($"Running {nameof(OnBeforeUninstall)}");
      base.OnBeforeUninstall(savedState);
      RunRegasm(Get32BitRuntimeDirectory(), true);
      RunRegasm(Get64BitRuntimeDirectory(), true);
    }

    private void RunRegasm(string regasmDir, bool withUninstallFlag)
    {
      Log($"Running {nameof(RunRegasm)} located in {regasmDir}");

      string targetDir = GetTargetInstallationDirectory();
      string shimDllPath = Path.Combine(targetDir, "vBase.dll");
      string shimTlbPath = Path.Combine(targetDir, "vBase.tlb");

      // run process regasm with /codebase option
      var process = new System.Diagnostics.Process();
      string regasmPath = Path.Combine(regasmDir, "regasm.exe");

      if (!File.Exists(regasmPath))
      {
        Log($"regasm {regasmPath} does not exist.");
      }

      process.StartInfo.FileName = regasmPath;
      process.StartInfo.Arguments =
        (withUninstallFlag ? "/u " : string.Empty) +
        $"/codebase \"{shimDllPath}\" /tlb:\"{shimTlbPath}\"";

      try
      {
        process.Start();
      }
      catch (Exception ex)
      {
        Log(ex.Message);
        throw;
      }
    }

    private static string Get64BitRuntimeDirectory()
    {
      if (Environment.Is64BitProcess)
      {
        return RuntimeEnvironment.GetRuntimeDirectory();
      }
      else
      {
        return RuntimeEnvironment.GetRuntimeDirectory().Replace("Framework", "Framework64");
      }
    }

    private static string Get32BitRuntimeDirectory()
    {
      if (Environment.Is64BitProcess)
      {
        return RuntimeEnvironment.GetRuntimeDirectory().Replace("Framework64", "Framework");
      }
      else
      {
        return RuntimeEnvironment.GetRuntimeDirectory();
      }
    }

    private string GetTargetInstallationDirectory()
    {
      string customActionAssemblyPath = this.Context.Parameters["assemblypath"];
      string targetDir = Path.GetDirectoryName(customActionAssemblyPath);
      return targetDir;
    }

    private void Log(string message)
    {
      string targetDir = GetTargetInstallationDirectory();
      string logPath = Path.Combine(targetDir, "ShimInstaller.log");
      File.AppendAllText(logPath, message + Environment.NewLine);
    }
  }
}
