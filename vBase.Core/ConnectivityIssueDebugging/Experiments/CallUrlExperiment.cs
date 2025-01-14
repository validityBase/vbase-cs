using System;
using System.Net;
using Microsoft.Extensions.Logging;

namespace vBase.Core.ConnectivityIssueDebugging.Experiments;

public abstract class CallUrlExperiment : Experiment
{
  private readonly string _url;
  private readonly HttpStatusCode _expectedCode;
  private readonly string _expectedContentPart;
  private readonly string _name;
  private readonly SecurityProtocolType? _securityProtocolType;
  private string _protocolUpdateStatus = String.Empty;

  public CallUrlExperiment(
    string name,
    SecurityProtocolType? securityProtocolType,
    ILogger logger,
    string url,
    HttpStatusCode expectedCode,
    string expectedContentPart) : base(logger)
  {
    _name = name;
    _url = url;
    _expectedCode = expectedCode;
    _expectedContentPart = expectedContentPart;
    _securityProtocolType = securityProtocolType;
  }

  public override string Name => $"{_name} | Protocol Update: {_protocolUpdateStatus}";

  public override string RunInternal(string id)
  {
    var oldProtocol = ServicePointManager.SecurityProtocol;
    try
    {
      try
      {
        if (_securityProtocolType.HasValue)
        {
          ServicePointManager.SecurityProtocol = _securityProtocolType.Value;
          _protocolUpdateStatus = "Success";
          _logger.LogInformation($"Security protocol set to: {_securityProtocolType}");
        }
        else
        {
          _protocolUpdateStatus = "Default";
        }
      }
      catch (Exception e)
      {
        _protocolUpdateStatus = "Failed";
        _logger.LogError(e, "Error setting protocol");
      }

      _logger.LogInformation($"Security protocol: {ServicePointManager.SecurityProtocol}");

      var res = CallUrl(_url);
      if (res.HttpStatusCode != _expectedCode)
      {
        throw new Exception($"Unexpected status code: {res.HttpStatusCode}");
      }

      if (!res.Content.Contains(_expectedContentPart))
      {
        throw new Exception($"Expected content part not found: {_expectedContentPart}");
      }

      return string.Empty;
    }
    finally
    {
      ServicePointManager.SecurityProtocol = oldProtocol;
    }
  }

  public abstract (HttpStatusCode HttpStatusCode, string Content) CallUrl(string url);
}