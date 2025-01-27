using System;
using System.Numerics;
using System.Threading.Tasks;

namespace vBase.Core;

/// <summary>
/// Common interface for commitment services.
/// </summary>
public interface ICommitmentService
{
  /// <summary>
  /// Current user account identifier.
  /// </summary>
  public string AccountIdentifier { get; }

  /// <summary>
  /// Checks if the specified object set exists.
  /// </summary>
  /// <param name="user">Set owner.</param>
  /// <param name="setCid">CID of the set.</param>
  Task<bool> UserSetExists(string user, Cid setCid);

  /// <summary>
  /// Checks whether the object with the specified CID was stamped at the given time.
  /// </summary>
  /// <param name="user">The address for the user who recorded the commitment.</param>
  /// <param name="objectCid">The CID identifying the object.</param>
  /// <param name="timestamp">The timestamp of the transaction.</param>
  /// <returns>True if the commitment has been verified successfully; False otherwise.</returns>
  Task<bool> VerifyUserObject(string user, Cid objectCid, DateTimeOffset timestamp);

  /// <summary>
  /// Verifies an object commitment previously recorded.
  /// </summary>
  /// <param name="user">The address for the user who recorded the commitment.</param>
  /// <param name="setCid">The CID for the set containing the object.</param>
  /// <param name="userSetObjectCidSum">The sum of all object hashes for the user set.</param>
  /// <returns>True if the commitment has been verified successfully; False otherwise.</returns>
  Task<bool> VerifyUserSetObjects(string user, Cid setCid, BigInteger userSetObjectCidSum);

  /// <summary>
  /// Records a set commitment.
  /// If the set already exists, no action will be taken.
  /// </summary>
  /// <param name="setCid">The CID identifying the set.</param>
  Task AddSet(Cid setCid);

  /// <summary>
  /// Adds an object CID to the specified set.
  /// </summary>
  /// <param name="setCid">CID of the set where the objectCid will be added.</param>
  /// <param name="objectCid">Object CID to add.</param>
  /// <returns>Receipt of the operation.</returns>
  Task<Receipt> AddSetObject(Cid setCid, Cid objectCid);
}