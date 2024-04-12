using JustProxies.Proxy.Core;
using JustProxies.RuleEngine.Core;
using JustProxies.RuleEngine.Exts;
using Microsoft.Extensions.Logging;

namespace JustProxies.RuleEngine;

public class Interceptor
{
    private readonly ILogger<Interceptor> _logger;
    private readonly IManagement _management;

    public Interceptor(ILogger<Interceptor> logger, IWebProxyServer server, IManagement management)
    {
        _logger = logger;
        _management = management;
        server.OnRequestReceived += Server_OnRequestReceived;
        server.OnResponseReceived += Server_OnResponseReceived;
    }

    private async Task Server_OnRequestReceived(IWebProxyServer server, WebProxyServerRequestReceivedEventArgs e)
    {
        var rules = _management.GetEnabledRules();
        var ruleData = rules.FirstOrDefault(p => p.IsMatch(e.HttpContext));
      
        await ruleData.Invoke(e.HttpContext);
    }

    private Task Server_OnResponseReceived(IWebProxyServer server, WebProxyServerResponseReceivedEventArgs e)
    {
        return Task.CompletedTask;
    }
}