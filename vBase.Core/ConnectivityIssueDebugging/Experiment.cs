using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace vBase.Core.ConnectivityIssueDebugging;

[DebuggerDisplay("{Name}")]
public abstract class Experiment
{
  protected ILogger _logger;
  public Experiment(ILogger logger)
  {
    _logger = logger;
  }
  public abstract string Name { get; }
  public virtual ExperimentSummary Run()
  {
    string correlationId = Guid.NewGuid().ToString();
    try
    {
      string data = RunInternal(correlationId);
      return new ExperimentSummary
      {
        CorrelationId = correlationId,
        Name = Name,
        IsSuccessful = true,
        CollectedData = data
      };
    }
    catch (Exception e)
    {
      return new ExperimentSummary
      {
        CorrelationId = correlationId,
        Name = Name,
        IsSuccessful = false,
        ErrorMessage = e.Message,
        Exception = e
      };
    }
  }

  public abstract string RunInternal(string id);

}