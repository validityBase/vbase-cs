using System;
using vBase.Core.Exceptions;

namespace vBase
{
  internal static class Utils
  {
    public static T PreprocessException<T>(Func<T> func)
    {
      try
      {
        return func();
      }
      catch (Exception ex)
      {
        throw PreprocessException(ex);
      }
    }

    public static void PreprocessException(Action action)
    {
      try
      {
        action();
      }
      catch (Exception ex)
      {
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
        $"\tError Type: {ex.GetType().FullName}\r\n" +
        $"\tStackTrace: {ex.StackTrace}", ex);
    }
  }
}
