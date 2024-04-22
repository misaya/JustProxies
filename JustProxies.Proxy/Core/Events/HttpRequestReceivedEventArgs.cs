namespace JustProxies.Proxy.Core.Events;

public record HttpRequestReceivedEventArgs
{
    public HttpRequestReceivedEventArgs(HttpContext context)
    {
        HttpContext = context;
    }

    public HttpContext HttpContext { get; private set; } = null!;
}