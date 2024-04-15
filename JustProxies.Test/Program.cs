using System.Net;
using JustProxies.Proxy;
using JustProxies.Proxy.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JustProxies.Test;

public class Program
{
    static async Task Main(string[] args)
    {
        var logger = new ConsoleLogger<WebProxyServer>();
        var options = new OptionsWrapper<WebProxyServerOptions>(new WebProxyServerOptions()
        {
            Address = "127.0.0.1",
            Port = 1234,
            NetworkInterface = "Wi-Fi"
        });
        var client = new HttpClient(new HttpClientHandler()
        {
            Proxy = new WebProxy("127.0.0.1:1234", true, ["https://*"]),
            UseProxy = true,
        });
        var server = new WebProxyServer(logger, options);
        server.OnRequestReceived += OnRequestReceived;
        await server.StartAsync(CancellationToken.None);
        while (Console.ReadLine() is { })
        {
            try
            {
                var result = await client.GetAsync("http://wiki.17usoft.com/");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    private static Task OnRequestReceived(IWebProxyServer server, WebProxyServerRequestReceivedEventArgs args)
    {
        return Task.CompletedTask;
    }
}