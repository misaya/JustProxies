using System.Diagnostics;
using System.Net;
using System.Text;
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
        client.Timeout = TimeSpan.FromSeconds(10000);
        var server = new WebProxyServer(logger, options);
        await server.StartAsync(CancellationToken.None);
        await Task.Delay(2000);
        try
        {
            for (int i = 0; i < 1000; i++)
            {
                var url = $"http://localhost:8880/hello/{i}/world/{i}";
                var text = await client.GetStringAsync(url);
                Console.WriteLine(text);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        Console.ReadLine();
    }
}