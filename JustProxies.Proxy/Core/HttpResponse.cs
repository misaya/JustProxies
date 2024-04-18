using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace JustProxies.Proxy.Core;

public class HttpResponse
{
    internal HttpResponse(Stream responseStream)
    {
        this.ResponseSteam = responseStream;
        Debug.WriteLine("ResponseSteam.CanRead:{0}", this.ResponseSteam.CanRead);
        Debug.WriteLine("ResponseSteam.CanWrite:{0}", this.ResponseSteam.CanWrite);
    }

    public Stream ResponseSteam { get; }

    public async Task WriteAsync(string data, Encoding encoding)
    {
        var bytes = encoding.GetBytes(data);
        await ResponseSteam.WriteAsync(bytes);
    }
}