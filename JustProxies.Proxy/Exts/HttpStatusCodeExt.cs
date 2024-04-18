using System.Net;
using System.Text;

namespace JustProxies.Proxy.Exts;

public static class HttpStatusCodeExt
{
    public static byte[] GetResponse(this HttpStatusCode? code)
    {
        if (code.HasValue)
        {
            var error = $"HTTP/1.0 {(int)code.Value} {code}\r\n";
            return Encoding.ASCII.GetBytes(error);
        }

        var error500 = $"HTTP/1.0 500 ProxyServerError.\r\n";
        return Encoding.ASCII.GetBytes(error500);
    }
}