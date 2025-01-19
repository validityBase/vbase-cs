using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.EIP712;
using Nethereum.Contracts;
using Nethereum.Signer.EIP712;
using Nethereum.Signer;
using Nethereum.Util;
using Newtonsoft.Json;
using vBase.Core.DTOs;
using vBase.Core.Exceptions;
using vBase.Core.Utilities;

namespace vBase.Core.Web3CommitmentService;

/// <summary>
/// Provides access to the CommitmentService smart contract over vBase forwarder.
/// </summary>
public class ForwarderCommitmentService: Web3CommitmentService
{
  private readonly Uri _forwarderUrl;
  private readonly string _apiKey;
  private SignatureDataDto? _signatureData;

  public ForwarderCommitmentService(string forwarderUrl, string apiKey, string privateKey, ILogger logger) : base(privateKey, logger)
  {
    if (string.IsNullOrWhiteSpace(forwarderUrl))
      throw new ArgumentException("Forwarder URL is required.", nameof(forwarderUrl));

    if (string.IsNullOrWhiteSpace(apiKey))
      throw new ArgumentException("API key is required.", nameof(apiKey));

    _forwarderUrl = new Uri(forwarderUrl);
    _apiKey = apiKey;
  }

  protected override async Task<ReceiptDto<ContractMethodExecuteResultDto>> CallContractFunction(Function function, string functionData)
  {
    Logger.LogInformation("Executing contract function via the forwarder.");

    await EnsureSignatureData();

    var metaTransactionTypedData = CreateMetaTransactionTypedData(functionData);
    var signedMetaTransactionTypedData = SignMetaTransactionTypedData(metaTransactionTypedData);

    var result = await CallForwarderApi<ReceiptDto<ContractMethodExecuteResultDto>>("execute", HttpMethod.Post,
      payload: new
      {
        ForwardRequest = new
        {
          From = Account.Address,
          // ReSharper disable once RedundantAnonymousTypePropertyName
          Nonce = _signatureData.AsserNotNull().Nonce,
          Data = functionData
        },
        Signature = signedMetaTransactionTypedData
      });

    _signatureData.AsserNotNull().Nonce++;

    return result;
  }

  protected override async Task<ReceiptDto<TResultType>> FetchStateVariable<TResultType>(string functionData)
  {
    var callParams = new Dictionary<string, string>
    {
      ["data"] = functionData
    };
    return await CallForwarderApi<ReceiptDto<TResultType>>("call", HttpMethod.Get, callParams);
  }

  private async Task EnsureSignatureData()
  {
    if (_signatureData == null)
    {
      Logger.LogInformation("Fetching signature data.");

      var receipt = await CallForwarderApi<ReceiptDto<SignatureDataDto>>("signature-data");
      if (!receipt.Success)
      {
        throw new vBaseException("Failed to get signature data from forwarder API");
      }
      _signatureData = receipt.Data;
    }
    else
    {
      Logger.LogInformation("Signature data is initialized and will be reused.");
    }
  }

  private async Task<TResult> CallForwarderApi<TResult>(
    string apiMethodName,
    HttpMethod? method = null,
    Dictionary<string, string>? requestParameters = null,
    object? payload = null)
  {

    Logger.LogInformation($"Calling forwarder API \"{apiMethodName}\".");

    requestParameters ??= new Dictionary<string, string>();
    requestParameters.Add("from", Account.Address);

    var apiUrl = Utils.BuildUri(_forwarderUrl, apiMethodName, requestParameters);

    var request = new HttpRequestMessage(method ?? HttpMethod.Get, apiUrl);
    request.Headers.Add("x-api-key", _apiKey);

    if (payload != null)
    {
      var jsonPayload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
      {
        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
      });
      request.Content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
    }

    using HttpClient client = new HttpClient();

    try
    {
      using var response = await client.SendAsync(request);

      Logger.LogInformation($"Forwarder API response status: {response.StatusCode}");

      var responseContent = await response.Content.ReadAsStringAsync();

      Logger.LogInformation($"Forwarder API response content: {responseContent}");

      try
      {
        response.EnsureSuccessStatusCode();
      }
      catch(HttpRequestException ex)
      {
        throw new vBaseException(
                   $"Failed to call forwarder API \"{apiMethodName}\". Status code: {response.StatusCode}. Response: {responseContent}",
                            ex);
      }

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
          $"Result received from forwarder API \"{apiMethodName}\" is not a valid JSON string. ({responseContent})",
          ex);
      }
    }
    catch (Exception)
    {
      // If the transaction failed, nonce may be invalid.
      // Force these to be reloaded on the next call.
      _signatureData = null;
      throw;
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
        new MemberValue { TypeName = "address", Value = Account.Address },
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
    var key = new EthECKey(Account.PrivateKey);
    try
    {
      EthECDSASignature signature = key.SignAndCalculateV(hashedData);
      string signatureStr = EthECDSASignature.CreateStringSignature(signature);
      return signatureStr;
    }
    catch (ArgumentOutOfRangeException ex)
    {
      throw new vBaseException($"Invalid format of the provided private key. Error: {ex.Message}", ex);
    }
  }
}