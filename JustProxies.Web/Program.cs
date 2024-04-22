using JustProxies.Proxy;
using JustProxies.Proxy.Core;
using JustProxies.Proxy.Core.Options;
using JustProxies.Proxy.Setup;
using JustProxies.Proxy.Setup.Core;
using JustProxies.RuleEngine;
using JustProxies.Web.Components;

namespace JustProxies.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);

        builder.Services.Configure<HttpProxyServerOptions>(
            builder.Configuration.GetSection(HttpProxyServerOptions.OptionsName));
        builder.Services.AddSingleton<IWebProxyTool, WebProxyTool>();
        builder.Services.AddSingleton<IHttpProxyServer, HttpProxyServer>();
        builder.Services.AddSingleton<Interceptor>();
        builder.Services.AddHostedService<IHttpProxyServer>(p => p.GetRequiredService<IHttpProxyServer>());

        builder.Services.AddBlazorBootstrap();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        var app = builder.Build();
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        app.Run();
    }
}