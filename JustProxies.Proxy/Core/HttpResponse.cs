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

    public async Task WriteAsync(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        await _stream.WriteAsync(bytes);
    }

    public async Task WriteAsync(byte[] bytes)
    {
        await _stream.WriteAsync(bytes);
    }

    public void Write(byte[] bytes)
    {
        _stream.Write(bytes);
    }

    public void Write(string text)
    {
        _stream.Write(Encoding.ASCII.GetBytes(text));
    }
}