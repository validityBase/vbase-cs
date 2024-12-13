using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Nethereum.ABI.EIP712;
using Nethereum.Contracts;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using vBase.Core.Base;
using vBase.Core.DTOs;
using vBase.Core.Exceptions;
using vBase.Core.Utilities;

namespace vBase.Core.CommunicationChannels;

public class ForwarderCommunicationChannel: ICommunicationChannel
{
  private readonly Uri _forwarderUrl;
  private readonly string _apiKey;
  private readonly Account _account;

  private SignatureDataDto? _signatureData;

  public ForwarderCommunicationChannel(string forwarderUrl, string apiKey, string privateKey)
  {
    _forwarderUrl = new Uri(forwarderUrl);
    _apiKey = apiKey;
    _account = new Account(privateKey);
  }

  public async Task<ReceiptDto<ContractMethodExecuteResultDto>> CallContractFunction(Function function, string functionData)
  {
    await EnsureSignatureData();

    var metaTransactionTypedData = CreateMetaTransactionTypedData(functionData);
    var signedMetaTransactionTypedData = SignMetaTransactionTypedData(metaTransactionTypedData);

    return await CallForwarderApi <ReceiptDto<ContractMethodExecuteResultDto>>("execute", HttpMethod.Post,
      new
      {
        ForwardRequest = new
        {
          From = _account.Address,
          _signatureData.AsserNotNull().Nonce,
          Data = functionData
        },
        Signature = signedMetaTransactionTypedData
      });
  }

  private async Task EnsureSignatureData()
  {
    if (_signatureData == null)
    {
      var receipt = await CallForwarderApi<ReceiptDto<SignatureDataDto>>("signature-data");
      if (!receipt.Success)
      {
        throw new vBaseException("Failed to get signature data from forwarder API");
      }
      _signatureData = receipt.Data;
    }
  }

  private async Task<TResult> CallForwarderApi<TResult>(string apiMethodName, HttpMethod? method = null, object? payload = null)
  {
    var requestParameters = new Dictionary<string, string>();
    requestParameters.Add("from", _account.Address);

    var apiUrl = Utils.BuildUri(_forwarderUrl, apiMethodName, requestParameters);

    var request = new HttpRequestMessage(method ?? HttpMethod.Get, apiUrl);
    request.Headers.Add("x-api-key", _apiKey);

    if(payload != null)
    {
      var jsonPayload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
      {
        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
      });
      request.Content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
    }

    using HttpClient client = new HttpClient();
    using var response = await client.SendAsync(request);

    var responseContent = await response.Content.ReadAsStringAsync();

    response.EnsureSuccessStatusCode();

    try
    {
      TResult? receipt = JsonConvert.DeserializeObject<TResult>(responseContent, new JsonSerializerSettings());
      if (receipt != null)
      {
        return receipt;
      }

      throw new vBaseException($"Unexpected response structure received from forwarder API {apiMethodName}");
    }
    catch (JsonReaderException ex)
    {
      throw new vBaseException(
        $"Result received from forwarder API {apiMethodName} is not a valid JSON string. ({responseContent})",
        ex);
    }
  }

  private TypedData<Domain> CreateMetaTransactionTypedData(string functionData)
  {
    if (_signatureData == null)
      throw new InvalidOperationException("Signature data is not initialized.");

    return new TypedData<Domain>
    {
      Domain = _signatureData.AsserNotNull().Domain,
      Types = new Dictionary<string, MemberDescription[]>
      {
        ["EIP712Domain"] =
        [
          new MemberDescription { Name = "name", Type = "string" },
          new MemberDescription { Name = "version", Type = "string" },
          new MemberDescription { Name = "chainId", Type = "uint256" },
          new MemberDescription { Name = "verifyingContract", Type = "address" }
        ],
        ["ForwardRequest"] =
        [
          new MemberDescription { Name = "from", Type = "address" },
          new MemberDescription { Name = "nonce", Type = "uint256" },
          new MemberDescription { Name = "data", Type = "bytes" }
        ]
      },
      PrimaryType = "ForwardRequest",
      Message =
      [
        new MemberValue { TypeName = "address", Value = _account.Address },
        new MemberValue { TypeName = "uint256", Value = _signatureData.Nonce },
        new MemberValue { TypeName = "bytes", Value = functionData }
      ]
    };
  }

  private string SignMetaTransactionTypedData(TypedData<Domain> typedData)
  {
    if (_signatureData == null)
      throw new InvalidOperationException("Signature data is not initialized.");

    Eip712TypedDataSigner signer = new Eip712TypedDataSigner();

    // Encode the typed data.
    byte[] encodedData = signer.EncodeTypedData(typedData);
    byte[] hashedData = Sha3Keccack.Current.CalculateHash(encodedData);

    // Sign the hash with the private key.
    var key = new EthECKey(_account.PrivateKey);
    EthECDSASignature signature = key.SignAndCalculateV(hashedData);
    string signatureStr = EthECDSASignature.CreateStringSignature(signature);

    return signatureStr;
  }
}