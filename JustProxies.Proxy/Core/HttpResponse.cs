using System.Net.Sockets;
using System.Text;

namespace JustProxies.Proxy.Core;

public class HttpResponse
{
    private readonly NetworkStream _stream;

    internal HttpResponse(NetworkStream stream)
    {
        this._stream = stream;
    }

    public Stream Stream => _stream;

    public async Task WriteAsync(string data, Encoding encoding)
    {
        var bytes = encoding.GetBytes(data);
        await _stream.WriteAsync(bytes);
    }

    public async Task WriteAsync(byte[] bytes)
    {
        await _stream.WriteAsync(bytes);
    }
}