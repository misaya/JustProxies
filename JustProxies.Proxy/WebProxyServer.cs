using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using JustProxies.Proxy.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JustProxies.Proxy;

//[DebuggerDisplay("Address={_options.Address};Port={_options.Port}")]
public class WebProxyServer : IWebProxyServer, IDisposable
{
    private readonly ILogger<WebProxyServer> _logger;
    private readonly WebProxyServerOptions _options;
    private readonly TcpListener _listener;

    public event WebProxyServerEventHandler? OnStarted;
    public event WebProxyServerEventHandler? OnStopped;
    public event WebProxyServerRequestReceived? OnRequestReceived;
    public event WebProxyServerResponseReceived? OnResponseReceived;

    public WebProxyServer(ILogger<WebProxyServer> logger, IOptions<WebProxyServerOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _options.ThrowExceptionIfInvalid();
        _listener = new TcpListener(_options.GetIPAddress(), _options.Port);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _listener.Start();
        OnStarted?.Invoke(this, WebProxyServerEventArgs.CreateStartedEvent(_options));
        Task.Factory.StartNew(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync(cancellationToken);
                var stream = tcpClient.GetStream();
                var total = new List<byte>();
                do
                {
                    var buffer = new byte[1024];
                    var length = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    total.AddRange(buffer.Take(length));
                } while (stream.DataAvailable);

                if (total.Count == 0)
                {
                    stream.Close();
                    tcpClient.Close();
                    continue;
                }

                try
                {
                    var httpContext = new HttpContext(tcpClient, total, stream);
                    await Process(httpContext);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError(e, "Error handling request.");
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(e.Message), cancellationToken);
                }
            }
        }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        _logger.LogInformation("Proxy server started.");
        this.OnStarted?.Invoke(this, WebProxyServerEventArgs.CreateStartedEvent(_options));
        return Task.CompletedTask;
    }

    private async Task Process(HttpContext context)
    {
        if (context.Request.RawUrl.Contains("fenghui.xu"))
        {
            context.Response.Write("hello,fenghui!");
            context.Close();
            return;
        }

        _logger.LogInformation("请求:\r\n" + Encoding.ASCII.GetString(context.Request.RawData.ToArray()));
        using var client = new TcpClient(context.Request.Url.Host, context.Request.Url.Port);
        await using var stream = client.GetStream();
        await stream.WriteAsync(context.Request.RawData.ToArray());
        var buffer = new byte[1024];
        var total = new List<byte>();
        do
        {
            var length = await stream.ReadAsync(buffer, 0, buffer.Length);
            total.AddRange(buffer.Take(length));
        } while (stream.DataAvailable);

        _logger.LogInformation("返回:" + total.ToArray().Length + " bytes");
        await context.Response.WriteAsync(total.ToArray());
        context.Close();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _listener.Stop();
        this.OnStopped?.Invoke(this, WebProxyServerEventArgs.CreateStoppedEvent(_options));
        return Task.CompletedTask;
    }


    public void Dispose()
    {
        _listener.Dispose();
    }
}