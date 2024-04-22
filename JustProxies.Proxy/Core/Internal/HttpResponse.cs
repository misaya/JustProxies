using System.Net.Sockets;
using JustProxies.Proxy.Exts;

namespace JustProxies.Proxy.Core.Internal;

public class HttpResponse
{
    private readonly Stream _responseSteam;

    internal HttpResponse(Stream responseStream)
    {
        _responseSteam = responseStream;
    }

    public Memory<byte> ResponseRawData { get; set; }
    public bool IsHandled { get; set; } = false;

    public async Task SubmitAsync()
    {
        if (ResponseRawData.IsEmpty || !_responseSteam.CanWrite) return;

        await _responseSteam.WriteAsync(ResponseRawData);
    }

    public void LinkExternalStream(NetworkStream stream)
    {
        if (_responseSteam.CanWrite)
        {
            var memoryStream = stream.ReadTo(_responseSteam);
            ResponseRawData = memoryStream.ToArray();
        }
    }
}