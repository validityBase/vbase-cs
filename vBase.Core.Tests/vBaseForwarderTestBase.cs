﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using vBase.Core.Utilities;
using vBase.Core.Web3CommitmentService;

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
      .AddYamlFile("settings.local.yml", true)
      .AddEnvironmentVariables()
    .Build();

    using ILoggerFactory factory = LoggerFactory.Create(_ => { });

    var commitmentService = new ForwarderCommitmentService(
      ForwarderUrl,
      ApiKey,
      PrivateKey,
      factory.CreateLogger(this.GetType())
    );

    Client = new vBaseClient(commitmentService);
  }
}