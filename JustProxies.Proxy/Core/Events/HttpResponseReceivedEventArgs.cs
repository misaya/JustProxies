using JustProxies.Proxy.Core.Internal;

namespace JustProxies.Proxy.Core.Events;

public record HttpResponseReceivedEventArgs
{
    public HttpResponseReceivedEventArgs(HttpContext context)
    {
        HttpContext = context;
    }

    public HttpContext HttpContext { get; private set; } = null!;
}