using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using vBase.Core.Dataset.vBaseObjects;
using vBase.Core.Exceptions;
using vBase.Core.Utilities;

namespace vBase.Core.Dataset;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class vBaseDataset
{
  private class Record
  {
    public vBaseObject vBaseObject { get; set; }
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
    _owner = vBaseClient.AccountIdentifier;

    if (!vBaseObjectFactory.IsTypeRegistered(recordTypeName))
    {
      throw new InvalidOperationException($"Unknown vBase object type {recordTypeName}.");
    }

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
      var dtoRecord = dto.Records[i];
      var data = vBaseObjectFactory.Create(dto.RecordTypeName);
      data.InitFromJson(dtoRecord.Data);

      // crosscheck
      if (dtoRecord.Cid != data.GetCid().ToHex())
      {
        throw new vBaseException($"Dataset loading error: CID mismatch for data {dtoRecord.Data} and CID {dtoRecord.Cid}");
      }

      _records.Add(new Record
      {
        vBaseObject = data,
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
    var obj = vBaseObjectFactory.Create(_recordTypeName, recordData);
    var timestamp = await _vBaseClient.AddSetObject(_name, obj.GetCid());
    _records.Add(new Record { vBaseObject = obj, Timestamp = timestamp });
  }

  public async Task<VerificationResult> VerifyCommitments()
  {
    var verificationResult = new VerificationResult();
    BigInteger objectCidSum = BigInteger.Zero;

    var maxSum = BigInteger.Pow(2, 256);

    foreach (var record in _records)
    {
      objectCidSum += record.vBaseObject.GetCid().CidToBigInt();
      objectCidSum %= maxSum;

      if (!await _vBaseClient.VerifyUserObject(_owner, record.vBaseObject.GetCid(), record.Timestamp))
      {
        verificationResult.AddFinding(
          $"""
           Invalid record:
           Failed object verification:
           Owner = {_owner},
           Timestamp = {record.Timestamp},
           ObjectCid = {record.vBaseObject.GetCid().ToHex()}
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
         SetCid = {_name.GetCid().ToHex()},
         ObjectCidSum = {objectCidSum.BigIntToEthereumBytes(256).ToHex()}
         """);
    }

    return verificationResult;
  }

  public string ToJson()
  {
    JsonSerializationDto serializationDto = new JsonSerializationDto
    {
      Name = _name,
      Cid = _name.GetCid().ToHex(),
      Owner = _owner,
      RecordTypeName = _recordTypeName,
      Records = _records.Select(r => new JsonSerializationRecord
      {
        Data = r.vBaseObject.GetJson(),
        Cid = r.vBaseObject.GetCid().ToHex()
      }).ToArray(),
      Timestamps = _records.Select(r => r.Timestamp).ToArray()
    };

    return Utils.SerializeObject(serializationDto);
  }
}