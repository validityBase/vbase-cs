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

/// <summary>
/// vBase dataset.
/// </summary>
public class vBaseDataset
{
  private class Record
  {
    public vBaseObject vBaseObject { get; set; }
    public DateTimeOffset Timestamp { get; set; }
  }

  private readonly vBaseClient _vBaseClient;
  private readonly string _name;
  private readonly Cid _setCid;
  private readonly string _owner;
  private readonly string _recordTypeName;
  private readonly List<Record> _records = [];

  /// <summary>
  /// Creates a new instance of the vBase dataset.
  /// </summary>
  /// <param name="vBaseClient">The vBaseClient used for communication with the vBase smart protocol.</param>
  /// <param name="name">The name of the dataset.</param>
  /// <param name="recordTypeName">The type of records to be stored in the dataset.</param>
  /// <exception cref="InvalidOperationException">Thrown if an unknown record type is provided.</exception>
  public vBaseDataset(vBaseClient vBaseClient, string name, string recordTypeName)
  {
    _vBaseClient = vBaseClient;
    _name = name;
    _setCid = name.GetCid();
    _owner = vBaseClient.DefaultUser;

    if (!vBaseObjectFactory.IsTypeRegistered(recordTypeName))
    {
      throw new InvalidOperationException($"Unknown vBase object type {recordTypeName}.");
    }

    _recordTypeName = recordTypeName;

    Initialize().Wait();
  }

  /// <summary>
  /// Creates a new instance of the vBase dataset from JSON.
  /// </summary>
  /// <param name="vBaseClient">The vBaseClient used for communication with the vBase smart protocol.</param>
  /// <param name="json">The JSON representation of the dataset. JSON created by vBase SDKs for other platforms, such as Python or Java, is also supported.</param>
  /// <exception cref="vBaseException">Thrown when the current CID generation algorithm does not match the one used to generate the provided JSON.</exception>
  public vBaseDataset(vBaseClient vBaseClient, string json)
  {
    _vBaseClient = vBaseClient;
    var dto = Utils.DeserializeObject<JsonSerializationDto>(json);
    _name = dto.Name;
    _setCid = dto.Name.GetCid();
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

  /// <summary>
  /// Creates a new dataset on the blockchain if it does not already exist.
  /// </summary>
  /// <returns>A task representing the asynchronous operation.</returns>
  public async Task Initialize()
  {
    if (!await _vBaseClient.UserNamedSetExists(_owner, _name))
    {
      await _vBaseClient.AddNamedSet(_name);
    }
  }

  /// <summary>
  /// Adds a record to the dataset.
  /// </summary>
  /// <param name="recordData">The record to add. The record type must match the dataset type.</param>
  /// <returns>A transaction receipt.</returns>
  public async Task<Receipt> AddRecord(object recordData)
  {
    var obj = vBaseObjectFactory.Create(_recordTypeName, recordData);
    var receipt = await _vBaseClient.AddSetObject(_setCid, obj.GetCid());
    _records.Add(new Record { vBaseObject = obj, Timestamp = receipt.Timestamp });
    return receipt;
  }

  /// <summary>
  /// Verifies if all records in the dataset were actually created on the Validity Base platform at the specified timestamps.
  /// </summary>
  /// <returns>
  /// Validation result: A collection of errors. For each record that was not found on the Validity Base platform, 
  /// or was added with a different timestamp, there will be a separate error item in the collection.
  /// Additionally, an error item will be added if the dataset on the Validity Base platform contains more records 
  /// than exist in this client-side dataset.
  /// </returns>
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

  /// <summary>
  /// Serializes the dataset into a vBase-compatible JSON representation.
  /// </summary>
  /// <returns>A JSON string.</returns>
  public string ToJson()
  {
    JsonSerializationDto serializationDto = new()
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