using System.Net;
using System.Net.Sockets;

namespace JustProxies.Proxy.Core.Internal;

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