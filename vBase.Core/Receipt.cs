using System;

namespace vBase.Core;

/// <summary>
/// Represents a transaction receipt.
/// </summary>
public class Receipt(DateTimeOffset vBaseTimestamp)
{
  public DateTimeOffset Timestamp { get; set; } = vBaseTimestamp;
}