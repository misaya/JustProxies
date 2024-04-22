using JustProxies.Proxy.Core.Events;
using Microsoft.Extensions.Hosting;

namespace JustProxies.Proxy.Core;

public interface IHttpProxyServer : IHostedService
{
    event HttpProxyServerEventHandler OnStarted;
    event HttpProxyServerEventHandler OnStopped;
    event HttpRequestReceived OnRequestReceived;
    event HttpResponseReceived OnResponseReceived;
}