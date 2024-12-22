using System.Runtime.InteropServices;

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
    /// <param name="datasetName">The name of the dataset.</param>
    /// <param name="record">The record to add.</param>
    /// <returns>The transaction timestamp of the record addition in Unix time format (seconds).</returns>
    long AddSetObject(string datasetName, object record);

    /// <summary>
    /// Checks if a named dataset exists.
    /// </summary>
    /// <param name="owner">The owner of the dataset.</param>
    /// <param name="datasetName">The name of the dataset.</param>
    /// <returns>True if the dataset exists; otherwise, False.</returns>
    bool UserNamedSetExists(string owner, string datasetName);

    /// <summary>
    /// Adds a named dataset. The dataset will be added for the account associated with the client object.
    /// </summary>
    /// <param name="datasetName">The name of the dataset to add.</param>
    void AddNamedSet(string datasetName);

    /// <summary>
    /// Verifies that the object with the given owner and CID was created.
    /// </summary>
    /// <param name="owner">The checksum address of the object's owner's account.</param>
    /// <param name="objectCid">The CID of the object.</param>
    /// <param name="timestamp">The timestamp of the object's creation, in Unix time format (seconds).</param>
    /// <returns>True if the object exists; otherwise, False.</returns>
    bool VerifyUserObject(string owner, byte[] objectCid, long timestamp);
  }
}