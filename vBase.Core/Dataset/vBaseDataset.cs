using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using vBase.Core.Utilities;

namespace vBase.Core.Dataset;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class vBaseDataset
{
  private class Record
  {
    public object Data { get; set; }
    public DateTimeOffset Timestamp { get; set; }
  }

  private readonly vBaseClient _vBaseClient;
  private readonly string _name;
  private readonly string _owner;
  private readonly string _recordTypeName;
  private readonly List<Record> _records = [];

  public vBaseDataset(vBaseClient vBaseClient, string name, string recordTypeName)
  {
    _vBaseClient = vBaseClient;
    _name = name;
    _owner = vBaseClient.Account.ChecksumAddress();
    _recordTypeName = recordTypeName;

    Initialize().Wait();
  }

  public vBaseDataset(vBaseClient vBaseClient, string json)
  {
    _vBaseClient = vBaseClient;
    var dto = Utils.DeserializeObject<JsonSerializationDto>(json);
    _name = dto.Name;
    _owner = dto.Owner;
    _recordTypeName = dto.RecordTypeName;

    for (int i = 0; i < dto.Records.Length; i++)
    {
      _records.Add(new Record
      {
        Data = StringToRecordData(dto.Records[i].Data, dto.RecordTypeName),
        Timestamp = dto.Timestamps[i]
      });
    }
  }

  public async Task Initialize()
  {
    if (!await _vBaseClient.UserNamedSetExists(_owner, _name))
    {
      await _vBaseClient.AddNamedSet(_name);
    }
  }

  public async Task AddRecord(object recordData)
  {
    if (recordData == null)
    {
      throw new ArgumentNullException(nameof(recordData));
    }

    var timestamp = await _vBaseClient.AddSetObject(_name, recordData);
    _records.Add(new Record { Data = recordData, Timestamp = timestamp });
  }

  public async Task<VerificationResult> VerifyCommitments()
  {
    var verificationResult = new VerificationResult();
    BigInteger objectCidSum = BigInteger.Zero;

    foreach (var record in _records)
    {
      objectCidSum = objectCidSum.Add(CryptoUtils.EthereumBytesToBigInt(record.Data.AsserNotNull()!.GetCid()));

      if (!await _vBaseClient.VerifyUserObject(_owner, record.Data.AsserNotNull()!.GetCid(), record.Timestamp))
      {
        verificationResult.AddFinding(
          $"""
           Invalid record:
           Failed object verification:
           Owner = {_owner},
           Timestamp = {record.Timestamp},
           ObjectCid = {record.Data.AsserNotNull()!.GetCid().ToHex(true)}
           """);
      }
    }

    if (!await _vBaseClient.VerifyUserSetObjects(_owner, _name.GetCid(), objectCidSum))
    {
      verificationResult.AddFinding(
        $"""
         Invalid records:
         Failed object set verification:
         Owner = {_owner},
         SetCid = {_name.GetCid().ToHex(true)},
         ObjectCidSum = {objectCidSum.BigIntToEthereumBytes(256).ToHex(true)}
         """);
    }

    return verificationResult;
  }

  public string ToJson()
  {
    JsonSerializationDto serializationDto = new JsonSerializationDto
    {
      Name = _name,
      Owner = _owner,
      RecordTypeName = _recordTypeName,
      Records = _records.Select(r => new JsonSerializationRecord { Data = RecordDataToString(r.Data!) }).ToArray(),
      Timestamps = _records.Select(r => r.Timestamp).ToArray()
    };

    return Utils.SerializeObject(serializationDto);
  }

  private string RecordDataToString(object data)
  {
    if (_recordTypeName == vBaseRecordTypes.vBaseStringObject)
    {
      return data.AsserNotNull()!.ToString();
    }

    throw new NotSupportedException($"Type {_recordTypeName} is not supported data type.");
  }

  private static object StringToRecordData(string data, string recordType)
  {
    if (recordType == vBaseRecordTypes.vBaseStringObject)
    {
      return data;
    }

    throw new NotSupportedException($"Type {recordType} is not supported data type.");
  }
}