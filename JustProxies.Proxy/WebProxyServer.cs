using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using JustProxies.Proxy.Core;
using JustProxies.Proxy.Exts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JustProxies.Proxy;

[DebuggerDisplay("Address={_options.Address};Port={_options.Port}")]
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
 
    public ConcurrentDictionary<int, TcpClient> ClientPool = new ConcurrentDictionary<int, TcpClient>();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _listener.Start();
        OnStarted?.Invoke(this, WebProxyServerEventArgs.CreateStartedEvent(_options));
        _logger?.LogInformation("正在启动服务.");
        var thread = new Thread(async () =>
        {
            _logger?.LogInformation("服务已启动.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync(cancellationToken);
                var hash = tcpClient.GetHashCode();
                this.ClientPool.TryAdd(hash, tcpClient);
                ThreadPool.QueueUserWorkItem(HandleTcpClient, hash);
            }
        });
        thread.Start();

        this.OnStarted?.Invoke(this, WebProxyServerEventArgs.CreateStartedEvent(_options));
        return Task.CompletedTask;
    }

    private async void HandleTcpClient(object? state)
    {
        if (state is not int key || !ClientPool.TryGetValue(key, out var tcpClient))
        {
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var from = tcpClient.Client.RemoteEndPoint;
        _logger?.LogInformation("#{key}->接收到来自{from}的请求", key, from);
        var stream = tcpClient.GetStream();
        try
        {
            var httpContext = new HttpContext(tcpClient);
            this.OnRequestReceived?.Invoke(this, new WebProxyServerRequestReceivedEventArgs(httpContext));
            if (!httpContext.Ishandled)
            {
                await DefaultHandle(httpContext);
            }
        }
        catch (HttpRequestException e)
        {
            var bytes = e.StatusCode.GetResponse();
            await stream.WriteAsync(bytes);
        }
        finally
        {
            stopwatch.Stop();
            _logger?.LogInformation("#{key}->完成到来自{from}的请求，耗时:{time}毫秒", key, from,
                stopwatch.Elapsed.TotalMilliseconds);
            await Task.Delay(5000);
            tcpClient.Close();
            tcpClient.Dispose();
        }
    }

    private async Task DefaultHandle(HttpContext httpContext)
    {
        using var client = new TcpClient(httpContext.Request.Url.Host, httpContext.Request.Url.Port);
        await using var stream = client.GetStream();
        await stream.WriteAsync(httpContext.Request.RequestRawData.ToArray());
        using var memoryStream = stream.ReadToMemoryStream();
        httpContext.Response.ResponseSteam.Write(memoryStream.ToArray());
        client.Close();
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