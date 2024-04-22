using JustProxies.Proxy.Core;
using JustProxies.Proxy.Core.Events;
using JustProxies.RuleEngine.Core;
using JustProxies.RuleEngine.Exts;
using Microsoft.Extensions.Logging;

namespace JustProxies.RuleEngine;

public class Interceptor
{
    private readonly ILogger<Interceptor> _logger;
    private readonly IManagement _management;

    public Interceptor(ILogger<Interceptor> logger, IHttpProxyServer server, IManagement management)
    {
        _logger = logger;
        _management = management;
        server.OnRequestReceived += Server_OnRequestReceived;
        server.OnResponseReceived += Server_OnResponseReceived;
    }

    private async Task Server_OnRequestReceived(IHttpProxyServer server, HttpRequestReceivedEventArgs e)
    {
        var packages = _management.GetEnabled();
        var package = packages.FirstOrDefault(p => p.IsMatch(e.HttpContext));
        if (package != null)
        {
            _logger.LogInformation($"来自{e.HttpContext.RemoteEndPoint}的请求{e.HttpContext.Request}被截获处理");
            await package.ApplyRuleAction(e.HttpContext);
        }
    }

    private Task Server_OnResponseReceived(IHttpProxyServer server, HttpResponseReceivedEventArgs e)
    {
        return Task.CompletedTask;
    }
}