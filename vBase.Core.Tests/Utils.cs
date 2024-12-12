using System.Text;
using Microsoft.Extensions.Configuration;

namespace vBase.Core.Tests;

public static class Utils
{
  public static IConfiguration YamlStringToConfiguration (string yaml)
  {
    using var inMemorySettings = new MemoryStream(Encoding.UTF8.GetBytes(yaml));
    return new ConfigurationBuilder()
        .AddYamlStream(inMemorySettings)
        .Build();
  }
}
