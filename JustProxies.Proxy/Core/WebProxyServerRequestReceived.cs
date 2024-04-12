namespace JustProxies.Proxy.Core;

public delegate Task WebProxyServerRequestReceived(IWebProxyServer webProxyServer,
    WebProxyServerRequestReceivedEventArgs e);