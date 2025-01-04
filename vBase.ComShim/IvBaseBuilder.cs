using System.Runtime.InteropServices;

namespace vBase
{
  /// <summary>
  /// COM does not support constructors with parameters, so we need to use a factory method to create the objects.
  /// </summary>
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [Guid(ComGuids.vBaseBuilderInterface)]
  public interface IvBaseBuilder
  {
    /// <summary>
    /// Create a COM visible client for the vBase API.
    /// </summary>
    /// <param name="forwarderUrl">Forwarder API url.</param>
    /// <param name="apiKey">API key.</param>
    /// <param name="privateKey">Private key.</param>
    /// <returns>Newly created client object.</returns>
    IvBaseClient CreateForwarderClient(string forwarderUrl, string apiKey, string privateKey);

    /// <summary>
    /// Create a COM visible dataset object.
    /// </summary>
    /// <param name="client">vBase client.</param>
    /// <param name="name">The name of the dataset.</param>
    /// <param name="objectType">Type of the objects that will be stored in the dataset.</param>
    /// <returns>Newly created dataset object.</returns>
    IvBaseDataset CreateDataset(IvBaseClient client, string name, ObjectTypes objectType);

    /// <summary>
    /// Create a COM visible dataset object.
    /// </summary>
    /// <param name="client">vBase client.</param>
    /// <param name="json">Json that contains all data records, and dataset properties.</param>
    /// <returns>Newly created dataset object.</returns>
    IvBaseDataset CreateDatasetFromJson(IvBaseClient client, string json);
  }
}