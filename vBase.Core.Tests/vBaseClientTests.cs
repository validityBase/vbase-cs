using FluentAssertions;
using vBase.Core.Exceptions;
using vBase.Core.Utilities;

namespace vBase.Core.Tests;

public class vBaseClientTests: vBaseForwarderTestBase
{
  [Test]
  public async Task UserNamedSetExists_SetDoesNotExistTest()
  {
    bool exists = await Client.UserNamedSetExists(
      Client.Account.ChecksumAddress(),
      TestContext.CurrentContext.Random.GetString(50));
    exists.Should().BeFalse();
  }

  [Test]
  public async Task AddNamedSetTest()
  {
    string setName = TestContext.CurrentContext.Random.GetString(50);

    bool existedBefore = await Client.UserNamedSetExists(Client.Account.ChecksumAddress(), setName);
    await Client.AddNamedSet(setName);
    bool existsAfter = await Client.UserNamedSetExists(Client.Account.ChecksumAddress(),setName);

    existedBefore.Should().BeFalse();
    existsAfter.Should().BeTrue();
  }

  [Test]
  public async Task ComplexScenario_HappyPathTest()
  {
    string setName = TestContext.CurrentContext.Random.GetString(50);
    await Client.AddNamedSet(setName);
    var objectToAdd = TestContext.CurrentContext.Random.GetString(50);

    // add and verify just added object
    var timestamp = await Client.AddSetObject(setName, objectToAdd);
    bool objectAdded = await Client.VerifyUserObject(
      Client.Account.ChecksumAddress(),
      objectToAdd.GetCid(), timestamp);
    objectAdded.Should().BeTrue();

    // verify object with invalid timestamp
    bool objectVerifiedWrongStamp = await Client.VerifyUserObject(
      Client.Account.ChecksumAddress(),
      objectToAdd.GetCid(), timestamp + TimeSpan.FromSeconds(10));
    objectVerifiedWrongStamp.Should().BeFalse();

    // verify object with invalid CID
    bool objectVerifiedWrongCid = await Client.VerifyUserObject(
      Client.Account.ChecksumAddress(),
      TestContext.CurrentContext.Random.GetString(50).GetCid(),
      timestamp);
    objectVerifiedWrongCid.Should().BeFalse();

    // verify set with 1 object
    bool setObjectsVerified = await Client.VerifyUserSetObjects(
      Client.Account.ChecksumAddress(),
      setName.GetCid(),
      CryptoUtils.EthereumBytesToBigInt(objectToAdd.GetCid())
    );
    setObjectsVerified.Should().BeTrue();

    // add one more object, and verify set of two records
    var objectToAdd2 = TestContext.CurrentContext.Random.GetString(50);
    await Client.AddSetObject(setName, objectToAdd2);
    setObjectsVerified = await Client.VerifyUserSetObjects(
      Client.Account.ChecksumAddress(),
      setName.GetCid(),
      CryptoUtils.EthereumBytesToBigInt(objectToAdd.GetCid())
        .Add(CryptoUtils.EthereumBytesToBigInt(objectToAdd2.GetCid()))
    );
    setObjectsVerified.Should().BeTrue();
  }

  [Test]
  public async Task AddSetObject_SetDoesNotExistTest()
  {
    string setName = TestContext.CurrentContext.Random.GetString(50);
    var action = async () => await Client.AddSetObject(setName, "ObjectToAdd");

    await action.Should()
      .ThrowAsync<vBaseException>()
      .WithMessage($"*Please make sure that the set with the specified name exists*");
  }
}