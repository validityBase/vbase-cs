using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
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

  public async Task<Dictionary<string, string>> CallMethod(string methodName, params object[] arguments)
  {
    await EnsureSignatureData();
    return new Dictionary<string, string>();
  }

  private async Task EnsureSignatureData()
  {
    _signatureData ??= await CallForwarderApi<SignatureDataDto>("signature-data");
  }

  private async Task<TData> CallForwarderApi<TData>(string apiMethodName)
  {
    var requestParameters = new Dictionary<string, string>();
    requestParameters.Add("from", _account.Address);

    var apiUrl = Utils.BuildUri(_forwarderUrl, apiMethodName, requestParameters);

    var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
    request.Headers.Add("x-api-key", _apiKey);

    using HttpClient client = new HttpClient();
    using var response = await client.SendAsync(request);

    var responseContent = await response.Content.ReadAsStringAsync();

    response.EnsureSuccessStatusCode();

    try
    {
      ReceiptDto<TData> receipt = JsonConvert.DeserializeObject<ReceiptDto<TData>>(responseContent, new JsonSerializerSettings())!;
      if (receipt != null)
      {
        if (!receipt.Success)
        {
          throw new vBaseException($"The Forwarder API {apiMethodName} did not return a result indicating success.");
        }

        return receipt.Data;
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

}