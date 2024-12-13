using FluentAssertions;
using Microsoft.Extensions.Configuration;
using vBase.Core.Utilities;

namespace vBase.Core.Tests;

[Category(Constants.TestCategoryIntegration)]
public class vBaseClientTests
{
  private vBaseClient _client;
  private IConfiguration _configuration;

  public string ForwarderUrl => _configuration[nameof(ForwarderUrl)].AsserNotNull();
  public string ApiKey => _configuration[nameof(ApiKey)].AsserNotNull();
  private string PrivateKey => _configuration[nameof(PrivateKey)].AsserNotNull();

  [SetUp]
  public void Setup()
  {
    _configuration = new ConfigurationBuilder()
      .AddYamlFile("settings.yml")
      .Build();

    var commitmentService = CommitmentServiceBuilder.BuildForwarderCommitmentService(
      ForwarderUrl,
      ApiKey,
      PrivateKey
    );

    _client = new vBaseClient(commitmentService);
  }

  [Test]
  public async Task UserNamedSetExistsTest()
  {
    bool exists = await _client.UserNamedSetExists(
      TestContext.CurrentContext.Random.GetString(50));
    exists.Should().BeFalse();
  }

  [Test]
  public async Task AddNamedSetTest()
  {
    string setName = TestContext.CurrentContext.Random.GetString(50);

    bool existedBefore = await _client.UserNamedSetExists(setName);
    await _client.AddNamedSet(setName);
    bool existsAfter = await _client.UserNamedSetExists(setName);

    existedBefore.Should().BeFalse();
    existsAfter.Should().BeTrue();
  }
}