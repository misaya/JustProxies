using System.Net.Sockets;
using System.Text;
using JustProxies.Common;
using JustProxies.Proxy;
using JustProxies.Proxy.Core;
using JustProxies.Proxy.Core.Events;
using JustProxies.Proxy.Core.Internal;
using JustProxies.Proxy.Exts;
using JustProxies.RuleEngine.Core;
using JustProxies.RuleEngine.Core.Models;
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
            await this.ApplyRuleAction(package, e.HttpContext);
        }
    }

    public override Task Server_OnResponseReceived(IHttpProxyServer server, HttpResponseReceivedEventArgs e)
    {
        var bytes = e.HttpContext.Response.ResponseRawData.ToArray();
        var text = Encoding.Default.GetString(bytes);
        logger.LogInformation($"来自{e.HttpContext.RemoteEndPoint}的请求{e.HttpContext.Request}被已响应:\r\n{text}");
        return Task.CompletedTask;
    }

    private async Task ApplyRuleAction(RulePackage package, HttpContext context)
    {
        switch (package.Action.ActionType)
        {
            case RuleActionType.UrlReWrite:
            {
                var newUrl = new Uri(package.Action.Resource["url"]);
                context.Request.UrlReWrite(newUrl);
                break;
            }
            case RuleActionType.CustomizeResponseContent:
            {
                const string firstLine = "HTTP/1.1 200 OK";
                var headers = new List<KeyValuePair<string, string>>
                {
                    (new("Proxy", "JustProxies")),
                    (new("ProxyRuleId", package.RuleId.ToString()))
                };
                var body = package.Action.Resource["body"];
                headers.Add(body.IsJson()
                    ? new KeyValuePair<string, string>("Content-Type", "application/json")
                    : new KeyValuePair<string, string>("Content-Type", "text/plain"));
                var contentLength = Encoding.Default.GetBytes(body).Length;
                headers.Add(new KeyValuePair<string, string>("Content-Length", contentLength.ToString()));
                var headerLine = string.Join("\n", headers.Select(x => $"{x.Key}: {x.Value}"));
                var bytes = Encoding.UTF8.GetBytes($"{firstLine}\n{headerLine}\n\n{body}");
                context.Response.ResponseRawData = bytes;
                context.Response.IsHandled = true;
                break;
            }
            case RuleActionType.ByPass:
            default:
                context.Response.IsHandled = false;
                break;
        }
    }
}