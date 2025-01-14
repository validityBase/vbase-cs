using System;
using System.IO;
using System.Net;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using vBase.Core.Utilities;

namespace vBase.Core.ConnectivityIssueDebugging.Experiments
{
  public class CallUrlOverWebClientExperiment : CallUrlExperiment
  {
    public CallUrlOverWebClientExperiment(
      string name,
      SecurityProtocolType? securityProtocolType,
      ILogger logger,
      string url,
      HttpStatusCode expectedCode,
      string expectedContentPart) : base(name, securityProtocolType, logger, url, expectedCode, expectedContentPart)
    {

    }

    public override (HttpStatusCode HttpStatusCode, string Content) CallUrl(string url)
    {
      try
      {
        var request = WebRequest.Create(url);
        var response = (HttpWebResponse)request.GetResponse();
        _logger.LogInformation($"Response status code: {response.StatusCode}");
        using StreamReader responseStreamReader = new StreamReader(response.GetResponseStream().AsserNotNull());
        var responseContent = responseStreamReader.ReadToEndAsync().Result;
        _logger.LogInformation($"Response content: {responseContent}");
        return (response.StatusCode, responseContent);
      }
      catch (WebException ex)
      {
        if (ex.Status == WebExceptionStatus.ProtocolError)
        {
          var response = ex.Response as HttpWebResponse;
          if (response != null)
          {
            _logger.LogInformation($"Response status code: {response.StatusCode}");
            using StreamReader responseStreamReader = new StreamReader(response.GetResponseStream().AsserNotNull());
            var responseContent = responseStreamReader.ReadToEndAsync().Result;
            _logger.LogInformation($"Response content: {responseContent}");
            return (response.StatusCode, responseContent);
          }
          else
          {
            return (0, string.Empty);
          }
        }
        else
        {
          return (0, string.Empty);
        }
      }
    }
  }
}
