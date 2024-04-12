namespace JustProxies.Proxy.Core;

public record WebProxyServerEventArgs
{
    private WebProxyServerEventArgs(WebProxyServerEventType eventType, WebProxyServerOptions options)
    {
        EventType = eventType;
        Options = options;
    }

    public WebProxyServerEventType EventType { get; private set; }

    public WebProxyServerOptions Options { get; private set; }

    public static WebProxyServerEventArgs CreateStoppedEvent(WebProxyServerOptions options)
    {
        return new WebProxyServerEventArgs(WebProxyServerEventType.Stopped, options);
    }

    public static WebProxyServerEventArgs CreateStartedEvent(WebProxyServerOptions options)
    {
        return new WebProxyServerEventArgs(WebProxyServerEventType.Started, options);
    }
}