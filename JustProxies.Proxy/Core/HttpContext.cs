using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace JustProxies.Proxy.Core;

public class HttpContext(TcpClient client, List<byte> totalBytes, NetworkStream stream)
{
    public HttpRequest Request { get; private set; } = new HttpRequest(client, totalBytes);
    public HttpResponse Response { get; private set; } = new HttpResponse(stream);

    public void Close()
    {
        stream.Close();
        client.Close();
    }
}