using System.Runtime.InteropServices;

namespace vBase
{
  /// <summary>
  /// Represents a set of records created on the Validity Base platform.
  /// </summary>
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [Guid(ComGuids.vBaseDatasetInterface)]
  public interface IvBaseDataset
  {

    /// <summary>
    /// Adds a record to the dataset.
    /// </summary>
    /// <param name="recordData">Record to add.</param>
    void AddRecord(object recordData);

    /// <summary>
    /// Verifies if all records in the dataset were actually created on the Validity Base platform at the specified timestamps.
    /// </summary>
    /// <returns>
    /// Validation result: A collection of errors. For each record that was not found on the Validity Base platform, 
    /// or was added with a different timestamp, there will be a separate error item in the collection.
    /// Additionally, an error item will be added if the dataset on the Validity Base platform contains more records 
    /// than exist in this client-side dataset.
    /// </returns>
    IVerificationResult VerifyCommitments();
  }
}