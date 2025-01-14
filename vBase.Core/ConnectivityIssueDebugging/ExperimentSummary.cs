using System;
using System.Diagnostics;

namespace vBase.Core.ConnectivityIssueDebugging
{
  [DebuggerDisplay("{Name} - {IsSuccessful}")]
  public class ExperimentSummary
  {
    public string CorrelationId { get; set; }
    public string Name { get; set; }
    public bool IsSuccessful { get; set; }
    public string CollectedData { get; set; }
    public string ErrorMessage { get; set; }
    public Exception? Exception { get; set; }
  }
}
