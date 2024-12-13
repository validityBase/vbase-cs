using FluentAssertions;

namespace vBase.Core.Tests;

[TestFixture("https://dev.api.vbase.com/forwarder/",
  "hPnKt94hz2CbZMmj6iz_4tWV0q21hQ3JOif02hOu6UU",
  "0x4d22553b6559103d337144874ce13489583de4a12516b0575840c0d6199cb296")]
public class vBaseClientTests(string ForwarderUrl, string ApiKey, string PrivateKey)
{
  private vBaseClient _client;

  [SetUp]
  public void Setup()
  {
    var commitmentService = CommitmentServiceBuilder.BuildForwarderCommitmentService(
      ForwarderUrl,
      ApiKey,
      PrivateKey
    );

    _client = new vBaseClient(commitmentService);
  }

  [Test]
  public async Task UserNamedSetExists_SentDoesNotExists_Test()
  {
    bool exists = await _client.UserNamedSetExists(
      TestContext.CurrentContext.Random.GetString(50));
    exists.Should().BeFalse();
  }

  [Test]
  public async Task AddNamedSet_Test()
  {
    string setName = TestContext.CurrentContext.Random.GetString(50);

    bool existedBefore = await _client.UserNamedSetExists(setName);
    await _client.AddNamedSet(setName);
    bool existsAfter = await _client.UserNamedSetExists(setName);

    existedBefore.Should().BeFalse();
    existsAfter.Should().BeTrue();
  }
}