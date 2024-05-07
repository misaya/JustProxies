using System.Net.Http.Headers;
using JustProxies.Common;

namespace JustProxies.Integration.SkyEye.Exts;

internal static class QueryRootExt
{
    internal static HttpContent GetHttpContent<T>(this T query) where T : SkyEyeQueriesRoot
    {
        if (query != null)
        {
            HttpContent content = new StringContent(query.ToJson(true));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            content.Headers.Add("token", string.Join(',', query.Tokens));
            return content;
        }

        return null!;
    }
}