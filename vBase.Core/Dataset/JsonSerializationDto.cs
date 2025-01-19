using System;
using Newtonsoft.Json.Linq;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace vBase.Core.Dataset;

/// <summary>
/// DTO for dataset JSON serialization.
/// It's important to keep this class in sync with the Python and other SDKs.
/// </summary>
internal class JsonSerializationDto
{
  public string Name { get; set; }

  public string Owner { get; set; }

  public string RecordTypeName { get; set; }

  public string Cid { get; set; }

  public JsonSerializationRecord[] Records { get; set; }

  public DateTimeOffset[] Timestamps { get; set; }
}

internal class JsonSerializationRecord
{
  public JValue? Data { get; set; }

  public string Cid { get; set; }
}