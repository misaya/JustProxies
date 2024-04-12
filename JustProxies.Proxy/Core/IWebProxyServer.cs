using Microsoft.Extensions.Hosting;

namespace JustProxies.Proxy.Core;

public interface IWebProxyServer : IHostedService
{
    event WebProxyServerEventHandler OnStarted;
    event WebProxyServerEventHandler OnStopped;
    event WebProxyServerRequestReceived OnRequestReceived;
    event WebProxyServerResponseReceived OnResponseReceived;
}