namespace vBase.Core.DTOs;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class DomainDto
{
  public string Name { get; set; }
  public string Version { get; set; }
  public string ChainId { get; set; }
  public string VerifyingContract { get; set; }
}