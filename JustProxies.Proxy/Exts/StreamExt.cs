using System.Net.Sockets;

namespace JustProxies.Proxy.Exts;

public static class StreamExt
{
    public static MemoryStream ReadAll(this NetworkStream networkStream)
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

    public static MemoryStream ReadTo(this NetworkStream networkStream, Stream stream)
    {
        var memoryStream = new MemoryStream();
        var buffer = new byte[512];
        do
        {
            var bytesRead = networkStream.Read(buffer);
            stream.Write(buffer, 0, bytesRead);
            memoryStream.Write(buffer, 0, bytesRead);
        } while (networkStream.DataAvailable);

        memoryStream.Position = 0;
        return memoryStream;
    }
}