using System.Net;
using System.Net.Sockets;
using JustProxies.Proxy.Core.Internal;

namespace JustProxies.Proxy.Core;

public class HttpContext
{
    public HttpContext(TcpClient client)
    {
        var stream = client.GetStream();
        Request = new HttpRequest(stream);
        Response = new HttpResponse(stream);
        RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint ?? null!;
    }

    public IPEndPoint RemoteEndPoint { get; }
    public HttpRequest Request { get; }
    public HttpResponse Response { get; }
}