using JustProxies.Proxy.Core.Options;

namespace JustProxies.Proxy.Core.Events;

public record HttpProxyServerEventArgs
{
    private HttpProxyServerEventArgs(HttpProxyServerEventType eventType)
    {
        EventType = eventType;
    }

    public HttpProxyServerEventType EventType { get; private set; }

    public static HttpProxyServerEventArgs CreateStoppedEvent()
    {
        return new HttpProxyServerEventArgs(HttpProxyServerEventType.Stopped);
    }

    public static HttpProxyServerEventArgs CreateStartedEvent()
    {
        return new HttpProxyServerEventArgs(HttpProxyServerEventType.Started);
    }
}