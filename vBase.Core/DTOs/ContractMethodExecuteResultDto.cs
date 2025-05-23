﻿using Newtonsoft.Json.Linq;

namespace vBase.Core.DTOs;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ContractMethodExecuteResultDto
{
  public string TransactionHash { get; set; }
  public JArray Logs { get; set; }
  public string Status { get; set; }
}