using NUnit.Framework.Internal;

namespace vBase.Core.Tests;

public class vBaseDatasetTests
{
  [Test]
  public async Task RunE2ETest()
  {
    try
    {


      var commitmentService = CommitmentServiceBuilder.BuildForwarderCommitmentService(
        "https://dev.api.vbase.com/forwarder/",
        "hPnKt94hz2CbZMmj6iz_4tWV0q21hQ3JOif02hOu6UU",
        "0x4d22553b6559103d337144874ce13489583de4a12516b0575840c0d6199cb296"
      );

      var client = new vBaseClient(commitmentService);

      await client.AddNamedSet("sddsfdqwd2223sd112ssdfdfdsa");

      //bool ds1 = await client.UserNamedSetExists("TestDataSet1111");

      //bool ds2 = await client.UserNamedSetExists("TestDataSet1111");
      //bool ds3 = await client.UserNamedSetExists("TestDataSet1111");
      //bool ds4 = await client.UserNamedSetExists("Test11212DataSet1111");
      //bool ds5 = await client.UserNamedSetExists("TestRecord");



      //var dataset = new vBaseDataset<string>(client, "TestDataSet");
      //var reciept = await dataset.AddRecord("TestRecord");

    }
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
      Assert.Fail();
    }
  }
}