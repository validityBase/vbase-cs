using System;
using System.IO;
using Microsoft.Extensions.Logging;
using vBase.Core.Exceptions;

namespace vBase
{
  /// <summary>
  /// Utility methods.
  /// </summary>
  internal static class Utils
  {
    /// <summary>
    /// Executes a function and logs any exceptions that occur.
    /// Additionally, converts the exception into a VBA-friendly exception.
    /// </summary>
    /// <typeparam name="T">Function return type.</typeparam>
    /// <param name="func">Function to execute.</param>
    /// <param name="logger">Logger.</param>
    /// <returns>Function execution result.</returns>
    public static T PreprocessException<T>(Func<T> func, ILogger logger = null)
    {
      try
      {
        return func();
      }
      catch (Exception ex)
      {
        logger?.LogError(ex, "An error occurred.");
        throw PreprocessException(ex);
      }
    }

    /// <summary>
    /// Executes an action and logs any exceptions that occur.
    /// Additionally, converts the exception into a VBA-friendly exception.
    /// </summary>
    /// <param name="action">Action to execute.</param>
    /// <param name="logger">Logger.</param>
    public static void PreprocessException(Action action, ILogger logger = null)
    {
      try
      {
        action();
      }
      catch (Exception ex)
      {
        logger?.LogError(ex, "An error occurred.");
        throw PreprocessException(ex);
      }
    }

    /// <summary>
    /// Converts a regular .NET exception into a VBA-friendly exception with all relevant information aggregated into the exception message.
    /// This improves the user experience in a VBA environment, where the error object does not include the stack trace or the original exception type.
    /// </summary>
    /// <param name="ex">The original exception.</param>
    /// <returns>A VBA-friendly exception with aggregated information.</returns>
    private static Exception PreprocessException(Exception ex)
    {
      // return unrecoverable exceptions as is
      if (ex is OutOfMemoryException || ex is StackOverflowException)
        return ex;

      // unwrap aggregate exceptions
      if (ex is AggregateException && ex.InnerException != null)
      {
        ex = ex.InnerException;
      }

      return new vBaseException(
        $"{ex.Message}\"\r\n" +
        "Additional Information:\r\n" +
        $"\tvBase SDK version: {typeof(Utils).Assembly.GetName().Version}\r\n" +
        $"\tLog File Location: {Path.Combine(Path.GetTempPath(), "vBase-logs*.txt")}\r\n" +
        $"\tError Type: {ex.GetType().FullName}\r\n" +
        $"\tStackTrace: {ex.StackTrace}", ex);
    }
  }
}
