namespace JustProxies.Proxy.Core;

public delegate Task WebProxyServerResponseReceived(IWebProxyServer webProxyServer,
    WebProxyServerResponseReceivedEventArgs e);