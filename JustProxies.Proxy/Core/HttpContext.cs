using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using JustProxies.Proxy.Exts;

namespace JustProxies.Proxy.Core;

public class HttpContext  
{
    public HttpContext(TcpClient client)
    {
        var stream = client.GetStream();
        this.Request = new HttpRequest(stream);
        this.Response = new HttpResponse(stream);
        this.RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint ?? null!;
    }

    public IPEndPoint RemoteEndPoint { get; }
    public HttpRequest Request { get; }
    public HttpResponse Response { get; }
    public bool Ishandled { get; set; } = false;
}