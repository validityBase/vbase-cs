using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Xml.Linq;
using ADRaffy.ENSNormalize;
using Microsoft.Extensions.Configuration;
using vBase.Core.Base;

namespace vBase.Core
{
  /// <summary>
  /// Provides Python validityBase (vBase) access.
  /// </summary>
  public class vBaseClient
  {
    private readonly CommitmentService _commitmentService;

    public vBaseClient(CommitmentService commitmentService)
    {
      _commitmentService = commitmentService;
    }

    public async Task<Dictionary<string, string>> AddSetObject(string cid, string recordCid)
    {
     return await _commitmentService.AddSetObject(cid, recordCid);
    }
  }
}
