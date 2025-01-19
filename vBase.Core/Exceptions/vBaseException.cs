using System;

namespace vBase.Core.Exceptions;

/// <summary>
/// Base exception for all vBase exceptions.
/// </summary>
public class vBaseException: Exception
{
  public vBaseException(string message)
    : base(message)
  {
  }

  public vBaseException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}