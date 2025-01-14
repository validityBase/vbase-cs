using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace vBase.Core.ConnectivityIssueDebugging.Experiments
{
  public class CallUrlOverHttpClientExperiment: CallUrlExperiment
  {
    public CallUrlOverHttpClientExperiment(
      string name,
      SecurityProtocolType? securityProtocolType,
      ILogger logger,
      string url,
      HttpStatusCode expectedCode,
      string expectedContentPart) : base(name, securityProtocolType,logger, url, expectedCode, expectedContentPart)
    {

    }

    public override (HttpStatusCode HttpStatusCode, string Content) CallUrl(string url)
    {
      using var request = new HttpRequestMessage(HttpMethod.Get, url);
      using var client = new HttpClient();
      using var response = client.SendAsync(request).Result;
      _logger.LogInformation($"Response status code: {response.StatusCode}");
      var responseContent = response.Content.ReadAsStringAsync().Result;
      _logger.LogInformation($"Response content: {responseContent}");
      return (response.StatusCode, responseContent);
    }
  }
}
