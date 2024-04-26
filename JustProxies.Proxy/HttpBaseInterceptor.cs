using JustProxies.Proxy.Core;
using JustProxies.Proxy.Core.Events;
using Microsoft.Extensions.Logging;

namespace JustProxies.Proxy;

public class HttpBaseInterceptor(ILogger<HttpBaseInterceptor> logger) : IHttpInterceptor
{
    public virtual Task Server_OnRequestReceived(IHttpProxyServer server, HttpRequestReceivedEventArgs e)
    {
        logger.LogInformation("收到来自{from}请求: {request}", e.HttpContext.RemoteEndPoint,
            e.HttpContext.Request.ToString());
        return Task.CompletedTask;
    }

    public virtual Task Server_OnResponseReceived(IHttpProxyServer server, HttpResponseReceivedEventArgs e)
    {
        logger.LogInformation("已处理返回");
        return Task.CompletedTask;
    }

    public virtual Task Server_Started(IHttpProxyServer server, HttpProxyServerEventArgs e)
    {
        logger.LogInformation("代理服务已启动,地址{address},端口{port}", server.Options.Address, server.Options.Port);
        return Task.CompletedTask;
    }

    public virtual Task Server_Stopped(IHttpProxyServer server, HttpProxyServerEventArgs e)
    {
        logger.LogInformation("服务已停止");
        return Task.CompletedTask;
    }
}