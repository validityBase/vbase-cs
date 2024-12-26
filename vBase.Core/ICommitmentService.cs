using System;
using System.Numerics;
using System.Threading.Tasks;

namespace vBase.Core;

public interface ICommitmentService
{
  public string AccountIdentifier { get; }

  /// <summary>
  /// Checks if the specified object set exists.
  /// </summary>
  /// <param name="owner">Set owner.</param>
  /// <param name="setName">Name of the set.</param>
  Task<bool> UserSetExists(string owner, string setName);

  /// <summary>
  /// Checks whether the object with the specified CID was stamped at the given time.
  /// </summary>
  /// <param name="owner">Object owner.</param>
  /// <param name="objectCid">The CID of the object.</param>
  /// <param name="timestamp">The timestamp of the transaction.</param>
  /// <returns>True if the object was stamped, otherwise false.</returns>
  Task<bool> VerifyUserObject(string owner, Cid objectCid, DateTimeOffset timestamp);

  /// <summary>
  /// Check the sum of the CIDs of the objects in the set.
  /// </summary>
  /// <param name="owner">Set owner.</param>
  /// <param name="setCid">Object CID.</param>
  /// <param name="setObjectsCidSum">The sum of the CIDs of the objects in the set.</param>
  /// <returns>True if the setObjectsCidSum match the sum of the CIDs of the objects in the set, otherwise false.</returns>
  Task<bool> VerifyUserSetObjects(string owner, Cid setCid, BigInteger setObjectsCidSum);

  /// <summary>
  /// Creates a new data set with the specified set name.
  /// If the set already exists, no action will be taken.
  /// </summary>
  /// <param name="setName">The name of the set to add.</param>
  Task AddSet(string setName);

  /// <summary>
  /// Adds an object CID to the specified set.
  /// </summary>
  /// <param name="setName">Name of the set where the objectCidToAdd will be added.</param>
  /// <param name="objectCidToAdd">Object CID to add.</param>
  Task<DateTimeOffset> AddSetObject(string setName, Cid objectCidToAdd);
}