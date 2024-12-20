using System;

namespace vBase.Core.Exceptions;

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