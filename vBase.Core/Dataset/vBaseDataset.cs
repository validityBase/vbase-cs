using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using vBase.Core.Utilities;

namespace vBase.Core.Dataset;

public class vBaseDataset<TDataType>
{
    private class Record
    {
        public TDataType? Data { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    private readonly vBaseClient _vBaseClient;
    private readonly string _name;
    private readonly string _owner;
    private readonly List<Record> _records = [];

    public vBaseDataset(vBaseClient vBaseClient, string name)
    {
        _vBaseClient = vBaseClient;
        _name = name;
        _owner = vBaseClient.Account.ChecksumAddress();

        Initialize().Wait();
    }

    public async Task Initialize()
    {
        if (!await _vBaseClient.UserNamedSetExists(_owner, _name))
        {
            await _vBaseClient.AddNamedSet(_name);
        }
    }

    public async Task AddRecord(TDataType recordData)
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
}