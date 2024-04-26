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
        var package = new RulePackage
        {
            RuleName = "返回helloworld",
            RuleDescription = "访问hello.com",
            IsEnabled = true,
            RuleId = Guid.NewGuid(),
            Action = new RuleAction()
            {
                ActionType = RuleActionType.ResponseCustomizeContent,
                Resource = "hello world~"
            },
            Rules =
            {
                Meet = RuleCondition.Any
            }
        };
        package.Rules.Add(new RuleItem
        {
            Target = RuleItemTarget.Url,
            Method = RuleItemMethod.Contains,
            RawData = "hello.com"
        });
        ruleManage.DeleteAll();
        ruleManage.Add(package);
        await host.StartAsync();
    }
}