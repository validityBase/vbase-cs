using Nethereum.ABI.EIP712;

namespace vBase.Core.DTOs;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class SignatureDataDto
{
  public int Nonce { get; set; }
  public Domain Domain { get; set; }
}