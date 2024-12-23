using System;
using Nethereum.Hex.HexConvertors.Extensions;
using Newtonsoft.Json;
using vBase.Core.Utilities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace vBase.Core.Dataset;

internal class JsonSerializationDto
{
  public string Name { get; set; }

  public string Owner { get; set; }

  public string RecordTypeName { get; set; }

  public string Cid => Name.GetCid().ToHex(true);

  public JsonSerializationRecord[] Records { get; set; }

  public DateTimeOffset[] Timestamps { get; set; }
}

internal class JsonSerializationRecord
{
  public string Data { get; set; }

  public string Cid => Data.GetCid().ToHex(true);
}