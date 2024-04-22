namespace JustProxies.Proxy.Core.Events;

public delegate Task HttpResponseReceived(IHttpProxyServer httpProxyServer,
    HttpResponseReceivedEventArgs e);