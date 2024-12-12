namespace vBase.Core.Tests;

public class vBaseClientTests
{
  [Test]
  public async Task RunE2ETest()
  {
    var commitmentService = CommitmentServiceBuilder.BuildForwarderCommitmentService(
      "https://dev.api.vbase.com/forwarder/",
      "hPnKt94hz2CbZMmj6iz_4tWV0q21hQ3JOif02hOu6UU",
      "0x4d22553b6559103d337144874ce13489583de4a12516b0575840c0d6199cb296"
    );

    var client = new vBaseClient(commitmentService);

    var dataset = new vBaseDataset<string>(client, "TestDataSet");

      

    var reciept = await dataset.AddRecord("TestRecord");
  }
}