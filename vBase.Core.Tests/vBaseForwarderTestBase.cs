using System.Collections;
using Microsoft.Extensions.Configuration;
using vBase.Core.Utilities;

namespace vBase.Core.Tests;

[Category(Constants.TestCategoryIntegration)]
public abstract class vBaseForwarderTestBase
{
  protected vBaseClient Client;
  private IConfiguration _configuration;

  public string ForwarderUrl => _configuration[nameof(ForwarderUrl)].AsserNotNull();
  public string ApiKey => _configuration[nameof(ApiKey)].AsserNotNull();
  private string PrivateKey => _configuration[nameof(PrivateKey)].AsserNotNull();

  [SetUp]
  public void Setup()
  {
    _configuration = new ConfigurationBuilder()
      .AddYamlFile("settings.yml")
      .AddEnvironmentVariables()
      .Build();

    Console.WriteLine("ENV _ VAR START");
    Console.WriteLine(Environment.GetEnvironmentVariable(nameof(ForwarderUrl)));
    Console.WriteLine(Environment.GetEnvironmentVariable(nameof(ApiKey)));
    Console.WriteLine(Environment.GetEnvironmentVariable(nameof(PrivateKey)));
    Console.WriteLine("ENV _ VAR END");

    var commitmentService = CommitmentServiceBuilder.BuildForwarderCommitmentService(
      ForwarderUrl,
      ApiKey,
      PrivateKey
    );

    Client = new vBaseClient(commitmentService);
  }
}