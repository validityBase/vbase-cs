using System.Numerics;
using FluentAssertions;
using vBase.Core.Dataset.vBaseObjects;
using vBase.Core.Exceptions;
using vBase.Core.Utilities;

namespace vBase.Core.Tests;

public class vBaseClientTests: vBaseForwarderTestBase
{
  [Test]
  public async Task UserNamedSetExists_SetDoesNotExistTest()
  {
    bool exists = await Client.UserNamedSetExists(
      Client.AccountIdentifier,
      TestContext.CurrentContext.Random.GetString(50));
    exists.Should().BeFalse();
  }

  [Test]
  public async Task AddNamedSetTest()
  {
    string setName = TestContext.CurrentContext.Random.GetString(50);

    bool existedBefore = await Client.UserNamedSetExists(Client.AccountIdentifier, setName);
    await Client.AddNamedSet(setName);
    bool existsAfter = await Client.UserNamedSetExists(Client.AccountIdentifier, setName);

    existedBefore.Should().BeFalse();
    existsAfter.Should().BeTrue();
  }

  [Test]
  public async Task ComplexScenario_HappyPathTest()
  {
    string setName = TestContext.CurrentContext.Random.GetString(50);
    await Client.AddNamedSet(setName);
    var objectToAdd = new vBaseStringObject(TestContext.CurrentContext.Random.GetString(50));

    // add and verify just added object
    var timestamp = await Client.AddSetObject(setName.GetCid(), objectToAdd.GetCid());
    bool objectAdded = await Client.VerifyUserObject(
      Client.AccountIdentifier,
      objectToAdd.GetCid(), timestamp);
    objectAdded.Should().BeTrue();

    // verify object with invalid timestamp
    bool objectVerifiedWrongStamp = await Client.VerifyUserObject(
      Client.AccountIdentifier,
      objectToAdd.GetCid(), timestamp + TimeSpan.FromSeconds(10));
    objectVerifiedWrongStamp.Should().BeFalse();

    // verify object with invalid CID
    bool objectVerifiedWrongCid = await Client.VerifyUserObject(
      Client.AccountIdentifier,
      TestContext.CurrentContext.Random.GetString(50).GetCid(),
      timestamp);
    objectVerifiedWrongCid.Should().BeFalse();

    // verify set with 1 object
    bool setObjectsVerified = await Client.VerifyUserSetObjects(
      Client.AccountIdentifier,
      setName.GetCid(),
      Utilities.Convert.EthereumBytesToBigInt(objectToAdd.GetCid().Data)
    );
    setObjectsVerified.Should().BeTrue();

    // add one more object, and verify set of two records
    var objectToAdd2 = new vBaseStringObject(TestContext.CurrentContext.Random.GetString(50));
    await Client.AddSetObject(setName.GetCid(), objectToAdd2.GetCid());
    var maxSum = BigInteger.Pow(2, 256);

    var sum = Utilities.Convert.EthereumBytesToBigInt(objectToAdd.GetCid().Data)
              +
              Utilities.Convert.EthereumBytesToBigInt(objectToAdd2.GetCid().Data);
    sum %= maxSum;

    setObjectsVerified = await Client.VerifyUserSetObjects(
      Client.AccountIdentifier,
      setName.GetCid(), sum);
    setObjectsVerified.Should().BeTrue();
  }

  [Test]
  public async Task AddSetObject_SetDoesNotExistTest()
  {
    string setName = TestContext.CurrentContext.Random.GetString(50);
    var action = async () => await Client.AddSetObject(setName.GetCid(), new vBaseStringObject("ObjectToAdd").GetCid());

    await action.Should()
      .ThrowAsync<vBaseException>()
      .WithMessage($"*Please make sure that the set with the specified name exists*");
  }
}