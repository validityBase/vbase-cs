using System.Runtime.InteropServices;
using vBase.Receipts;

namespace vBase
{
  /// <summary>
  /// COM visible client interface for the vBase API.
  /// It's a shim between the COM client and the vBase.Core client class.
  /// </summary>
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [Guid(ComGuids.vBaseClientInterface)]
  public interface IvBaseClient
  {
    /// <summary>
    /// Adds a record to the dataset.
    /// </summary>
    /// <param name="setCid">HEX encoded CID for the set containing the object.</param>
    /// <param name="objectCid">HEX encoded CID of the object to record.</param>
    /// <returns>The transaction timestamp of the record addition in Unix time format (seconds).</returns>
    IReceipt AddSetObject(string setCid, string objectCid);

    /// <summary>
    /// Checks if a named dataset exists.
    /// </summary>
    /// <param name="user">The address for the user who recorded the commitment.</param>
    /// <param name="name">The name of the set.</param>
    /// <returns>True if the set with the given name exists; False otherwise.</returns>
    bool UserNamedSetExists(string user, string name);

    /// <summary>
    /// Creates a commitment for a set with a given name.
    /// The set will be added for the account associated with the client object.
    /// </summary>
    /// <param name="name">The name of the set.</param>
    void AddNamedSet(string name);

    /// <summary>
    /// Records a set commitment.
    /// </summary>
    /// <param name="setCid">The HEX encoded CID (hash) identifying the set.</param>
    void AddSet(string setCid);

    /// <summary>
    /// Verifies an object commitment previously recorded.
    /// </summary>
    /// <param name="user">The address for the user who recorded the commitment.</param>
    /// <param name="objectCid">The HEX encoded CID of the object.</param>
    /// <param name="timestamp">The timestamp of the object's creation, in Unix time format (seconds).</param>
    /// <returns>True if the commitment has been verified successfully; False otherwise.</returns>
    bool VerifyUserObject(string user, string objectCid, long timestamp);

    /// <summary>
    /// Verifies an object commitment previously recorded.
    /// </summary>
    /// <param name="user">The address for the user who recorded the commitment.</param>
    /// <param name="setCid">The CID for the set containing the object.</param>
    /// <param name="userSetObjectsCidSum">The sum of all object hashes for the user set.</param>
    /// <returns>True if the commitment has been verified successfully; False otherwise.</returns>
    bool VerifyUserSetObjects(string user, string setCid, string userSetObjectsCidSum);
  }
}