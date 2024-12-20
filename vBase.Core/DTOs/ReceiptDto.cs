namespace vBase.Core.DTOs;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ReceiptDto<TData>
{
  public bool Success { get; set; }
  public TData Data { get; set; }
}