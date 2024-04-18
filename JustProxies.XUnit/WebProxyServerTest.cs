using System.Net;
using JustProxies.Proxy;
using JustProxies.Proxy.Core;
using JustProxies.Test;
using Microsoft.Extensions.Options;

namespace JustProxies.XUnit;

public class WebProxyServerTest
{
    [Fact]
    public async Task TestRequestProxy()
    {
        var logger = new ConsoleLogger<WebProxyServer>();
        var options = new OptionsWrapper<WebProxyServerOptions>(new WebProxyServerOptions()
        {
            Address = "127.0.0.1",
            Port = 1234,
            NetworkInterface = "Wi-Fi"
        });
        var webServerHost = "http://127.0.0.1:" + Random.Shared.Next(8000, 9999) + "/";
        var webServer = new WebServer();
        webServer.Start(webServerHost);
        var proxyServer = new WebProxyServer(logger, options);
        await proxyServer.StartAsync(CancellationToken.None);
        var client = new HttpClient(new HttpClientHandler()
        {
            Proxy = new WebProxy(options.Value.Address + ":" + options.Value.Port, true, ["https://*"]),
            UseProxy = true,
        });
        client.Timeout = TimeSpan.FromSeconds(10000);
        for (var i = 0; i < 1000; i++)
        {
            var url = $"{webServerHost}hello/{i}/world/{i}";
            var text = await client.GetStringAsync(url);
            Assert.EndsWith(text, url);
        }

        webServer.Stop();
        await proxyServer.StopAsync(CancellationToken.None);
    }
}