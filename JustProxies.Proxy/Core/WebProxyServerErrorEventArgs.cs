namespace JustProxies.Proxy.Core;

public record WebProxyServerErrorEventArgs
{
    public WebProxyServerEventType EventType => WebProxyServerEventType.Error;

    private WebProxyServerErrorEventArgs(Exception ex)
    {
        Exception = ex;
    }

    public Exception Exception { get; private set; }

    public static WebProxyServerErrorEventArgs CreateErrorEvent(Exception ex)
    {
        return new WebProxyServerErrorEventArgs(ex);
    }
}