using System.Text;
using JustProxies.Proxy.Core.Internal;
using JustProxies.RuleEngine.Core.Models;

namespace JustProxies.RuleEngine.Exts;

public static class RulePackageExt
{
    public static async Task ApplyRuleAction(this RulePackage package, HttpContext context)
    {
        switch (package.Action.ActionType)
        {
            case RuleActionType.Default:
                context.Response.IsHandled = false;
                break;
            case RuleActionType.ProxyRedirect:
            {
                var newUrl = new Uri(package.Action.Resource);
                using var client = new HttpClient();
                using var message = context.Request.GetHttpRequestMessage();
                message.Headers.Add("Proxy", "JustProxies");
                message.RequestUri = newUrl;
                var resp = await client.SendAsync(message);
                var bytes = await resp.Content.ReadAsByteArrayAsync();
                context.Response.ResponseRawData = bytes;
                context.Response.IsHandled = true;
                break;
            }
            case RuleActionType.ResponseCustomizeContent:
            {
                var firstLine =
                    $"HTTP/1.1 200 OK\nProxy:JustProxies\nProxyRule:{package.RuleId}\nProxyName:{package.RuleName}\n";
                var text = firstLine + "\n" + package.Action.Resource;

                var bytes = Encoding.UTF8.GetBytes(text);
                context.Response.ResponseRawData = bytes;
                context.Response.IsHandled = true;
                break;
            }
            default:
                context.Response.IsHandled = false;
                break;
        }
    }
}