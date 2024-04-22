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
        var rules = _management.GetEnabledRules();
        var ruleData = rules.FirstOrDefault(p => p.IsMatch(e.HttpContext));
        if (ruleData != null)
        {
            _logger.LogInformation($"来自{e.HttpContext.RemoteEndPoint}的请求{e.HttpContext.Request.ToString()}被截获处理");
            await ruleData.Invoke(e.HttpContext);
        }
    }

    private Task Server_OnResponseReceived(IHttpProxyServer server, HttpResponseReceivedEventArgs e)
    {
        return Task.CompletedTask;
    }
}