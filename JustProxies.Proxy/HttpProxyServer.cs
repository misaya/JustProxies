using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
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

    public HttpProxyServerOptions Options { get; private set; }

    public HttpProxyServer(ILogger<HttpProxyServer> logger, IOptions<HttpProxyServerOptions> options,
        IHttpInterceptor interceptor)
    {
        _logger = logger;
        this.Options = options.Value;
        this.Options.ThrowExceptionIfInvalid();
        _listener = new TcpListener(this.Options.GetIPAddress(), this.Options.Port);
        if (interceptor != null)
        {
            this.OnRequestReceived += interceptor.Server_OnRequestReceived;
            this.OnResponseReceived += interceptor.Server_OnResponseReceived;
            this.OnStarted += interceptor.Server_Started;
            this.OnStopped += interceptor.Server_Stopped;
        }
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

        var stream = tcpClient.GetStream();

        try
        {
            var httpContext = new HttpContext(tcpClient);
            try
            {
                await ProcessOn(key, httpContext);
            }
            catch (HttpRequestException e)
            {
                var bytes = e.StatusCode.GetResponse();
                await stream.WriteAsync(bytes);
                var text = e.StatusCode.GetResponseText();
                _logger.LogError(e, "来自{from}的请求处理过程发生异常,已返回内容:{text}", from, text);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "来自{from}的请求处理过程发生异常", from);
            }
            finally
            {
                //todo: 将context记录到日志系统
                stopwatch.Stop();
                tcpClient.Close();
                tcpClient.Dispose();
            }
        }
        catch (Exception e)
        {
            throw new HttpRequestException(HttpRequestError.ConnectionError, e.Message, e, HttpStatusCode.BadRequest);
        }
    }

    private async Task ProcessOn(int key, HttpContext httpContext)
    {
        OnRequestReceived?.Invoke(this, new HttpRequestReceivedEventArgs(httpContext));
        if (!httpContext.Response.IsHandled)
        {
            try
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
            catch (SocketException e)
            {
                _logger.LogError(e, "代理处理请求过程发生异常, {text}, 错误代码{errorCode},网路错误代码{socketErrorCode}", e.Message,
                    e.ErrorCode,
                    e.SocketErrorCode.ToString());
                throw new HttpRequestException(HttpRequestError.ConnectionError, e.Message, e,
                    HttpStatusCode.GatewayTimeout);
            }
        }

        OnResponseReceived?.Invoke(this, new HttpResponseReceivedEventArgs(httpContext));
        await httpContext.Response.SubmitAsync();
    }
}