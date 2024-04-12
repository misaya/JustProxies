using System.Net;

namespace JustProxies.Proxy.Core;

public record WebProxyServerRequestReceivedEventArgs
{
    public HttpContext HttpContext { get; private set; } = null!;

    public WebProxyServerRequestReceivedEventArgs(HttpContext context)
    {
        this.HttpContext = context;
    }
}