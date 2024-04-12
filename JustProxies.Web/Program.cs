using JustProxies.Proxy;
using JustProxies.Proxy.Core;
using JustProxies.Proxy.Setup;
using JustProxies.Proxy.Setup.Core;
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

        builder.Services.Configure<WebProxyServerOptions>(
            builder.Configuration.GetSection(WebProxyServerOptions.OptionsName));
        builder.Services.AddSingleton<IWebProxyTool, WebProxyTool>();
        builder.Services.AddSingleton<IWebProxyServer, WebProxyServer>();
        //  builder.Services.AddSingleton<Interceptor>();
        builder.Services.AddHostedService<IWebProxyServer>(p => p.GetRequiredService<IWebProxyServer>());

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