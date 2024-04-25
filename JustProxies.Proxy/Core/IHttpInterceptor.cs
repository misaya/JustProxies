using JustProxies.Proxy.Core.Events;

namespace JustProxies.Proxy.Core;

public interface IHttpInterceptor
{
    Task Server_OnRequestReceived(IHttpProxyServer server, HttpRequestReceivedEventArgs e);
    Task Server_OnResponseReceived(IHttpProxyServer server, HttpResponseReceivedEventArgs e);
    Task Server_Started(IHttpProxyServer server, HttpProxyServerEventArgs e);
    Task Server_Stopped(IHttpProxyServer server, HttpProxyServerEventArgs e);
}