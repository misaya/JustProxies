using System.Text;
using JustProxies.Proxy;
using JustProxies.Proxy.Core;
using JustProxies.Proxy.Core.Events;
using JustProxies.RuleEngine.Core;
using JustProxies.RuleEngine.Exts;
using Microsoft.Extensions.Logging;

namespace JustProxies.RuleEngine;

public class HttpInterceptor(ILogger<HttpInterceptor> logger, IRuleManagement ruleManagement)
    : HttpBaseInterceptor(logger)
{
    public override async Task Server_OnRequestReceived(IHttpProxyServer server, HttpRequestReceivedEventArgs e)
    {
        var packages = ruleManagement.GetEnabled();
        var package = packages.FirstOrDefault(p => p.IsMatch(e.HttpContext));
        if (package != null)
        {
            logger.LogInformation(
                $"来自{e.HttpContext.RemoteEndPoint}的请求{e.HttpContext.Request}被截获处理, 执行{package.RuleName}({package.RuleDescription})(#{package.RuleId})");
            await package.ApplyRuleAction(e.HttpContext);
        }
    }

    public override Task Server_OnResponseReceived(IHttpProxyServer server, HttpResponseReceivedEventArgs e)
    {
        var bytes = e.HttpContext.Response.ResponseRawData.ToArray();
        var text = Encoding.Default.GetString(bytes);
        logger.LogInformation($"来自{e.HttpContext.RemoteEndPoint}的请求{e.HttpContext.Request}被已响应:\r\n{text}");
        return Task.CompletedTask;
    }
}