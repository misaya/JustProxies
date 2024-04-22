using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using JustProxies.Proxy.Core;
using JustProxies.Proxy.Core.Events;
using JustProxies.Proxy.Core.Internal;
using JustProxies.Proxy.Core.Options;
using JustProxies.Proxy.Exts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JustProxies.Proxy;

public class HttpProxyServer : IHttpProxyServer, IDisposable
{
    private readonly ConcurrentDictionary<int, TcpClient> _clientPool = new();
    private readonly TcpListener _listener;
    private readonly ILogger<HttpProxyServer> _logger;

    public HttpProxyServer(ILogger<HttpProxyServer> logger, IOptions<HttpProxyServerOptions> options)
    {
        _logger = logger;
        var optionsValue = options.Value;
        optionsValue.ThrowExceptionIfInvalid();
        _listener = new TcpListener(optionsValue.GetIPAddress(), optionsValue.Port);
    }

    public void Dispose()
    {
        _listener.Dispose();
    }

    public event HttpProxyServerEventHandler? OnStarted;
    public event HttpProxyServerEventHandler? OnStopped;
    public event HttpRequestReceived? OnRequestReceived;
    public event HttpResponseReceived? OnResponseReceived;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _listener.Start();
        var thread = new Thread(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var tcpClient = _listener.AcceptTcpClient();
                var hash = tcpClient.GetHashCode();
                _clientPool.TryAdd(hash, tcpClient);
                ThreadPool.QueueUserWorkItem(HandleTcpClient, hash);
            }
        });
        thread.Start();
        OnStarted?.Invoke(this, HttpProxyServerEventArgs.CreateStartedEvent());
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _listener.Stop();
        OnStopped?.Invoke(this, HttpProxyServerEventArgs.CreateStoppedEvent());
        return Task.CompletedTask;
    }

    private async void HandleTcpClient(object? state)
    {
        if (state is not int key || !_clientPool.TryGetValue(key, out var tcpClient)) return;

        var stopwatch = Stopwatch.StartNew();
        var from = tcpClient.Client.RemoteEndPoint;
        _logger?.LogInformation("#{key}->接收到来自{from}的请求", key, from);
        var stream = tcpClient.GetStream();
        try
        {
            var httpContext = new HttpContext(tcpClient);
            await ProcessOn(httpContext);
        }
        catch (HttpRequestException e)
        {
            var bytes = e.StatusCode.GetResponse();
            await stream.WriteAsync(bytes);
            var text = e.StatusCode.GetResponseText();

            _logger?.LogWarning("#{key}->拒绝到来自{from}的请求,返回:{text}", key, from, text);
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

    private async Task ProcessOn(HttpContext httpContext)
    {
        OnRequestReceived?.Invoke(this, new HttpRequestReceivedEventArgs(httpContext));
        if (!httpContext.Response.IsHandled)
        {
            using var client = new TcpClient(httpContext.Request.Url.Host, httpContext.Request.Url.Port);
            await using var stream = client.GetStream();
            await stream.WriteAsync(httpContext.Request.RequestRawData.ToArray());
            //没有拦截，则实时的传输流，不经过内存缓存，提交性能；
            if (OnResponseReceived == null)
            {
                httpContext.Response.LinkExternalStream(stream);
                httpContext.Response.IsHandled = true;
                client.Close();
                return;
            }

            //存在拦截，则先将数据读取到缓冲区中，触发事件（事件中允许对缓存区数据进行篡改），最终传输到原客户端流；
            using var memoryStream = stream.ReadAll();
            httpContext.Response.ResponseRawData = memoryStream.ToArray();
            httpContext.Response.IsHandled = true;
            client.Close();
        }

        OnResponseReceived?.Invoke(this, new HttpResponseReceivedEventArgs(httpContext));
        await httpContext.Response.SubmitAsync();
    }
}