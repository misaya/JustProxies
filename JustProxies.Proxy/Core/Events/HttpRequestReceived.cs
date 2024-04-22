namespace JustProxies.Proxy.Core.Events;

public delegate Task HttpRequestReceived(IHttpProxyServer httpProxyServer,
    HttpRequestReceivedEventArgs e);