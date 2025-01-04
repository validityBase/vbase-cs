using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using NetEscapades.Extensions.Logging.RollingFile;
using vBase.Core;
using vBase.Core.Web3CommitmentService;
using vBase.Infrastructure;

namespace vBase
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid(ComGuids.vBaseBuilder)]
  public class vBaseBuilder: IvBaseBuilder
  {
    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    static vBaseBuilder()
    {
      AssemblyResolver.Register();
    }

    public vBaseBuilder()
    {
      Utils.PreprocessException(() =>
      {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
          builder.AddFile(options =>
          {
            options.LogDirectory = Path.GetTempPath(); // log to the system temp directory
            options.FileName = "vBase-logs"; // log file name prefix
            options.RetainedFileCountLimit = 10; // keep up to 10 log files
            options.Periodicity = PeriodicityOptions.Daily; // roll log files daily
          });
        });

        _logger = _loggerFactory.CreateLogger<vBaseBuilder>();
      });
    }

    public IvBaseClient CreateForwarderClient(string forwarderUrl, string apiKey, string privateKey)
    {
      return Utils.PreprocessException(() =>
      {
        ICommitmentService commitmentService = new ForwarderCommitmentService(
          forwarderUrl, apiKey, privateKey, _loggerFactory.CreateLogger(typeof(ForwarderCommitmentService)));

        return new vBaseClient(commitmentService, _loggerFactory.CreateLogger(typeof(vBaseClient)));
      }, _logger);
    }

    public IvBaseDataset CreateDataset(IvBaseClient client, string name, ObjectTypes objectType)
    {
      return Utils.PreprocessException(() => 
        new vBaseDataset(
          client, name, objectType, _loggerFactory.CreateLogger(typeof(vBaseDataset)) ), _logger);
    }

    public IvBaseDataset CreateDatasetFromJson(IvBaseClient client, string json)
    {
      return Utils.PreprocessException(() => new vBaseDataset(client, json), _logger);
    }
  }
}