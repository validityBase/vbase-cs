using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace vBase.Core.Utilities;

public static class Utils
{
  public static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new()
  {
    ContractResolver = new CamelCasePropertyNamesContractResolver
    {
      NamingStrategy = new SnakeCaseNamingStrategy(),
    },
    DateFormatString = "yyyy-MM-dd HH:mm:sszzz"
  };

  public static string SerializeObject(object @object)
  {
    return JsonConvert.SerializeObject(@object, DefaultJsonSerializerSettings);
  }

  public static T DeserializeObject<T>(string json)
  {
    return JsonConvert.DeserializeObject<T>(json, DefaultJsonSerializerSettings).AsserNotNull();
  }

  public static T AsserNotNull<T>(this T? value, string? message = null)
  {
    if (value == null)
    {
      throw new InvalidOperationException(message);
    }

    return value;
  }

  public static Uri BuildUri(Uri baseUri, string path, Dictionary<string, string> queryParams)
  {
    UriBuilder uriBuilder = new(baseUri);
    uriBuilder.Path = $"{uriBuilder.Path.TrimEnd('/')}/{path.TrimStart('/')}";

    string queryString = string.Join("&", queryParams
      .Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value.ToString())}"));

    uriBuilder.Query = queryString;
    return uriBuilder.Uri;
  }

  public static string LoadEmbeddedJson(string path)
  {
    var assembly = typeof(Utils).Assembly;
    var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(path));
    if (resourceName == null)
    {
      throw new InvalidOperationException($"Resource {path} not found in assembly {assembly.FullName}");
    }

    using var stream = assembly.GetManifestResourceStream(resourceName);
    using var reader = new System.IO.StreamReader(stream.AsserNotNull());
    return reader.ReadToEnd();
  }

  public static T GetEventParameterValue<T>(this EventLog<List<ParameterOutput>> @event, string paramName)
  {
    var param = @event.Event
      .SingleOrDefault(p => p.Parameter.Name == paramName);

    if (param == null)
    {
      throw new InvalidOperationException($"Parameter {paramName} not found in event");
    }

    return (T)param.Result;
  }
}