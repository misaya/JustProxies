using JustProxies.Proxy.Core.Events;
using JustProxies.Proxy.Core.Options;
using Microsoft.Extensions.Hosting;

namespace JustProxies.Proxy.Core;

public interface IHttpProxyServer : IHostedService
{
    event HttpProxyServerEventHandler OnStarted;
    event HttpProxyServerEventHandler OnStopped;
    event HttpRequestReceived OnRequestReceived;
    event HttpResponseReceived OnResponseReceived;

    HttpProxyServerOptions Options { get; }
}