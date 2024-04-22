namespace JustProxies.Proxy.Core.Events;

public delegate Task HttpProxyServerEventHandler(IHttpProxyServer httpProxyServer, HttpProxyServerEventArgs e);