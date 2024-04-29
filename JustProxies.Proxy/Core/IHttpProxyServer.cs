using JustProxies.Proxy.Core.Events;
using JustProxies.Proxy.Core.Options;
using Microsoft.Extensions.Hosting;

namespace JustProxies.Proxy.Core;

public interface IHttpProxyServer : IHostedService
{
    HttpProxyServerOptions Options { get; }
}