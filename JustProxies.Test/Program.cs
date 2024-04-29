using JustProxies.Common;
using JustProxies.Proxy;
using JustProxies.Proxy.Core;
using JustProxies.Proxy.Core.Options;
using JustProxies.Proxy.Setup;
using JustProxies.Proxy.Setup.Core;
using JustProxies.RuleEngine;
using JustProxies.RuleEngine.Core;
using JustProxies.RuleEngine.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JustProxies.Test;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
        var section = builder.Configuration.GetSection(HttpProxyServerOptions.OptionsName);
        builder.Services.Configure<HttpProxyServerOptions>(section);

        builder.Services.AddSingleton<IHttpProxyTool, HttpProxyTool>();
        builder.Services.AddSingleton<IHttpProxyServer, HttpProxyServer>();
        builder.Services.AddSingleton<IRuleManagement, RuleManagement>();
        builder.Services.AddSingleton<IHttpInterceptor, HttpInterceptor>();
        builder.Services.AddHostedService<IHttpProxyServer>(p => p.GetRequiredService<IHttpProxyServer>());
        var host = builder.Build();

        var ruleManage = host.Services.GetRequiredService<IRuleManagement>();
        var helloWorldPackage = new RulePackage
        {
            RuleName = "拦截测试",
            RuleDescription = "访问hello.com",
            IsEnabled = true,
            RuleId = Guid.NewGuid(),
            Action = new RuleAction()
            {
                ActionType = RuleActionType.CustomizeResponseContent,
                Resource = new RuleActionResource() { Body = new { Message = "helloworld" }.ToJson() }
            },
            Rules =
            {
                Match = RuleMatch.Any
            }
        };
        helloWorldPackage.Rules.Add(new RuleItem
        {
            Target = RuleMatchTarget.Url,
            Method = RuleMatchMethod.Contains,
            RawData = "hello.com"
        });
        var redirectPackage = new RulePackage
        {
            RuleName = "重定向测试",
            RuleDescription = "访问world.com",
            IsEnabled = true,
            RuleId = Guid.NewGuid(),
            Action = new RuleAction()
            {
                ActionType = RuleActionType.UrlReWrite,
                Resource = new RuleActionResource() { Url = "http://finance.fly.17usoft.com/clear/swagger/v1/swagger.json" }
            },
            Rules =
            {
                Match = RuleMatch.Any
            }
        };
        redirectPackage.Rules.Add(new RuleItem
        {
            Target = RuleMatchTarget.Url,
            Method = RuleMatchMethod.Contains,
            RawData = "world.com"
        });
        ruleManage.DeleteAll();
        ruleManage.Add(helloWorldPackage);
        ruleManage.Add(redirectPackage);
        await host.StartAsync();
    }
}