using JustProxies.Integration.Core;
using JustProxies.Integration.SkyEye;
using JustProxies.Proxy;
using JustProxies.Proxy.Core;
using JustProxies.Proxy.Core.Options;
using JustProxies.Proxy.Setup;
using JustProxies.Proxy.Setup.Core;
using JustProxies.RuleEngine;
using JustProxies.RuleEngine.Core;
using JustProxies.Web.Components;

namespace JustProxies.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region 处理IOptions配置
        builder.Configuration
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
        builder.Services.Configure<HttpProxyServerOptions>(
            builder.Configuration.GetSection(HttpProxyServerOptions.OptionsName));
        builder.Services.Configure<SkyEyeOptions>(
            builder.Configuration.GetSection(SkyEyeOptions.OptionsName));
        #endregion
        
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<ISkyEyeIntegration, SkyEyeIntegration>();
        builder.Services.AddSingleton<IHttpProxyTool, HttpProxyTool>();
        builder.Services.AddSingleton<IRuleManagement, RuleManagement>();
        builder.Services.AddSingleton<IHttpInterceptor, HttpInterceptor>();
        builder.Services.AddSingleton<IHttpProxyServer, HttpProxyServer>();
        builder.Services.AddHostedService<IHttpProxyServer>(p => p.GetRequiredService<IHttpProxyServer>());
       
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