using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vBase.Core.Utilities;

public static class Utils
{
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
        UriBuilder uriBuilder = new UriBuilder(baseUri);
        uriBuilder.Path = $"{uriBuilder.Path.TrimEnd('/')}/{path.TrimStart('/')}";

        string queryString = string.Join("&", queryParams
          .Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value.ToString())}"));

        uriBuilder.Query = queryString;
        return uriBuilder.Uri;
    }
}