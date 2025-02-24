using System;
using System.Numerics;
using System.Threading.Tasks;
using vBase.Core.Utilities;

namespace vBase.Core
{
  /// <summary>
  /// Provides Python validityBase (vBase) access.
  /// </summary>
  public class vBaseClient
  {
    private readonly ICommitmentService _commitmentService;

    public vBaseClient(ICommitmentService commitmentService)
    {
      _commitmentService = commitmentService;
    }

    /// <summary>
    /// Return the default user address used in vBase transactions.
    /// </summary>
    public string DefaultUser => _commitmentService.DefaultUser;

    /// <summary>
    /// Adds a new object to the set.
    /// </summary>
    /// <param name="setCid">Set CID.</param>
    /// <param name="objectCid">Object to add CID.</param>
    /// <returns>Receipt of the operation.</returns>
    public async Task<Receipt> AddSetObject(Cid setCid, Cid objectCid)
    {
      return await _commitmentService.AddSetObject(setCid, objectCid);
    }

    /// <summary>
    /// Checks if the user has a set with the specified CID.
    /// </summary>
    /// <param name="user">User's identifier.</param>
    /// <param name="name">Name of the set.</param>
    /// <returns></returns>
    public async Task<bool> UserNamedSetExists(string user, string name)
    {
      return await _commitmentService.UserSetExists(user, name.GetCid());
    }

    /// <summary>
    /// Adds a new named set.
    /// </summary>
    /// <param name="name">The name of the set to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddNamedSet(string name)
    {
      await _commitmentService.AddSet(name.GetCid());
    }

    /// <summary>
    /// Adds a new set.
    /// </summary>
    /// <param name="setCid">The identifier of the set.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddSet(Cid setCid)
    {
      await _commitmentService.AddSet(setCid);
    }

    /// <summary>
    /// Verifies if the specified object was stamped at the given time by the given user.
    /// </summary>
    /// <param name="user">The object owner.</param>
    /// <param name="objectCid">The object identifier.</param>
    /// <param name="timestamp">The time when the object was stamped.</param>
    /// <returns>True if the object was stamped; otherwise, false.</returns>
    public async Task<bool> VerifyUserObject(string user, Cid objectCid, DateTimeOffset timestamp)
    {
      return await _commitmentService.VerifyUserObject(user, objectCid, timestamp);
    }

    /// <summary>
    /// Verifies if the sum of all CIDs in the current dataset matches the sum of the dataset stored
    /// in the commitment service.
    /// </summary>
    /// <param name="user">The set owner.</param>
    /// <param name="setCid">The CID of the set.</param>
    /// <param name="userSetObjectsCidSum">The sum of the CIDs of all objects belonging to the set.</param>
    /// <returns>A boolean indicating whether the sums match.</returns>
    public async Task<bool> VerifyUserSetObjects(string user, Cid setCid, BigInteger userSetObjectsCidSum)
    {
      return await _commitmentService.VerifyUserSetObjects(user, setCid, userSetObjectsCidSum);
    }
  }
}
