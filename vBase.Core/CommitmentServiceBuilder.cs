using vBase.Core.Base;
using vBase.Core.CommunicationChannels;

namespace vBase.Core;

public class CommitmentServiceBuilder
{
  public static CommitmentService BuildForwarderCommitmentService(string forwarderUrl, string apiKey, string privateKey)
  {
    return new CommitmentService(
      new ForwarderCommunicationChannel(forwarderUrl, apiKey, privateKey)
    );
  }
}