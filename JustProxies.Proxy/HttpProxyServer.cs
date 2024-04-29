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
    private readonly IHttpInterceptor _interceptor;
    public HttpProxyServerOptions Options { get; }

    public HttpProxyServer(ILogger<HttpProxyServer> logger, IOptions<HttpProxyServerOptions> options,
        IHttpInterceptor interceptor)
    {
        _logger = logger;
        _interceptor = interceptor;
        this.Options = options.Value;
        this.Options.ThrowExceptionIfInvalid();
        _listener = new TcpListener(this.Options.GetIPAddress(), this.Options.Port);
    }

    public void Dispose()
    {
        _listener.Dispose();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
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
        await _interceptor.Server_Started(this, HttpProxyServerEventArgs.CreateStartedEvent());
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _listener.Stop();
        await _interceptor.Server_Stopped(this, HttpProxyServerEventArgs.CreateStoppedEvent());
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
        await _interceptor.Server_OnRequestReceived(this, new HttpRequestReceivedEventArgs(httpContext));
        if (!httpContext.Response.IsHandled)
        {
            try
            {
                using var client = new TcpClient(httpContext.Request.Url.Host, httpContext.Request.Url.Port);
                await using var stream = client.GetStream();
                await stream.WriteAsync(httpContext.Request.TotalContent.ToArray());
                if (!this.Options.EnableBuffer)
                {
                    httpContext.Response.LinkExternalStream(stream);
                    httpContext.Response.IsHandled = true;
                    client.Close();
                    return;
                }

                using var memoryStream = stream.ReadResponse();
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

        await _interceptor.Server_OnResponseReceived(this, new HttpResponseReceivedEventArgs(httpContext));
        await httpContext.Response.SubmitAsync();
    }
}