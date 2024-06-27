using JustProxies.Blazor.Components;
using JustProxies.Proxy;
using JustProxies.Proxy.Core;
using JustProxies.Proxy.Core.Options;
using JustProxies.Proxy.Setup;
using JustProxies.Proxy.Setup.Core;
using JustProxies.RuleEngine;
using JustProxies.RuleEngine.Core;

var builder = WebApplication.CreateBuilder(args);

#region 处理IOptions配置

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
builder.Services.Configure<HttpProxyServerOptions>(
    builder.Configuration.GetSection(HttpProxyServerOptions.OptionsName));

#endregion

#region 处理依赖注入

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpProxyTool, HttpProxyTool>();
builder.Services.AddSingleton<IRuleManagement, RuleManagement>();
builder.Services.AddSingleton<IHttpInterceptor, HttpInterceptor>();
builder.Services.AddSingleton<IHttpProxyServer, HttpProxyServer>();

#endregion

builder.Services.AddHostedService<IHttpProxyServer>(p => p.GetRequiredService<IHttpProxyServer>());
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBootstrapBlazor();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();