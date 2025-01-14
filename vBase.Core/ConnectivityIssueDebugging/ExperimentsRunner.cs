using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using vBase.Core.ConnectivityIssueDebugging.Experiments;

namespace vBase.Core.ConnectivityIssueDebugging
{
  public class ExperimentsRunner
  {
    private List<Experiment> _experiments = new List<Experiment>();
    private ILogger _logger;

    public ExperimentsRunner(ILogger logger)
    {
      AddCallExperimentsForProtocol(null, _experiments, logger);
      AddCallExperimentsForProtocol(0, _experiments, logger);
      AddCallExperimentsForProtocol(SecurityProtocolType.Tls, _experiments, logger);
      AddCallExperimentsForProtocol(SecurityProtocolType.Tls11, _experiments, logger);
      AddCallExperimentsForProtocol(SecurityProtocolType.Tls12, _experiments, logger);
      AddCallExperimentsForProtocol(SecurityProtocolType.Ssl3, _experiments, logger);

      _logger = logger;
    }

    private static void AddCallExperimentsForProtocol(SecurityProtocolType? securityProtocolType, List<Experiment> experiments, ILogger logger)
    {
      string protocol = securityProtocolType == null ? "Default" : securityProtocolType.ToString();

      experiments.Add(new CallUrlOverHttpClientExperiment(
        $"Call Weather API With Http Client and {protocol} Sec Protocol",
        securityProtocolType,
        logger,
        "https://api.openweathermap.org/data/2.5/weather?lat=44.34&lon=10.99",
        HttpStatusCode.Unauthorized,
        "Invalid API key"));

      experiments.Add(new CallUrlOverWebClientExperiment(
        $"Call Weather API With Web Client and {protocol} Sec Protocol",
        securityProtocolType,
        logger,
        "https://api.openweathermap.org/data/2.5/weather?lat=44.34&lon=10.99",
        HttpStatusCode.Unauthorized,
        "Invalid API key"));


      experiments.Add(new CallUrlOverHttpClientExperiment(
        $"Call Forwarder API With Http Client and {protocol} Sec Protocol",
        securityProtocolType,
        logger,
        "https://dev.api.vbase.com/forwarder",
        HttpStatusCode.Unauthorized,
        "timestamp"));

      experiments.Add(new CallUrlOverWebClientExperiment(
        $"Call Forwarder API With Web Client and {protocol} Sec Protocol",
        securityProtocolType,
        logger,
        "https://dev.api.vbase.com/forwarder",
        HttpStatusCode.Unauthorized,
        "timestamp"));

    }

    public void RunExperiments()
    {
      string sessionId = Guid.NewGuid().ToString();
      _logger.LogInformation($"Running experiments session [{sessionId}]...");

      _logger.LogInformation($"System Information:");
      _logger.LogInformation($"OS: {Environment.OSVersion}");
      _logger.LogInformation($"Runtime Version: {Environment.Version}");
      _logger.LogInformation($"64-bit OS: {Environment.Is64BitOperatingSystem}");
      _logger.LogInformation($"64-bit Process: {Environment.Is64BitProcess}");


      List<ExperimentSummary> summaries = new List<ExperimentSummary>();
      foreach (Experiment experiment in _experiments)
      {
        summaries.Add(experiment.Run());
      }

      _logger.LogInformation($"Experiments summary:");
      foreach (ExperimentSummary summary in summaries)
      {
        _logger.LogInformation($"=========================================");
        _logger.LogInformation($"Correlation ID [{summary.CorrelationId}]");
        _logger.LogInformation($"Experiment Name [{summary.Name}]");
        _logger.LogInformation($"Experiment Status: {summary.IsSuccessful}");
        _logger.LogInformation($"Collected Data: {summary.CollectedData}");
        _logger.LogInformation($"Error Message: {summary.ErrorMessage}");
        if (summary.Exception != null)
        {
          _logger.LogError(summary.Exception, "Experiment exception");
        }
        _logger.LogInformation($"=========================================");
      }

      _logger.LogInformation($"Experiments session [{sessionId}] completed.");
    }
  }
}
