using System.Diagnostics;
using System.Net.Sockets;

namespace JustProxies.Proxy.Exts;

public static class StreamExt
{
    public static MemoryStream ReadToMemoryStream(this NetworkStream networkStream)
    {
        var memoryStream = new MemoryStream();
        var buffer = new byte[512];
        do
        {
            var bytesRead = networkStream.Read(buffer);
            memoryStream.Write(buffer, 0, bytesRead);
        } while (networkStream.DataAvailable);

        memoryStream.Position = 0;
        return memoryStream;
    }
}