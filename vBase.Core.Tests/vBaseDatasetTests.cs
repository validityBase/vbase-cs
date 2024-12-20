using FluentAssertions;
using vBase.Core.Dataset;

namespace vBase.Core.Tests
{
  public class vBaseDatasetTests: vBaseForwarderTestBase
  {
    [Test]
    public async Task AddVerify_ComplexScenario_HappyPath_Test()
    {
      var dataset = new vBaseDataset<string>(Client, 
        TestContext.CurrentContext.Random.GetString(50));

      await dataset.AddRecord(TestContext.CurrentContext.Random.GetString(50));
      await dataset.AddRecord(TestContext.CurrentContext.Random.GetString(50));

      var verificationResult = await dataset.VerifyCommitments();
      verificationResult.VerificationPassed.Should().BeTrue();
    }
  }
}
