using System.Diagnostics;
using System.Net.Sockets;

namespace JustProxies.Proxy.Exts;

public static class StreamExt
{
    public static MemoryStream ReadToMemoryStream(this NetworkStream networkStream)
    {
        var memoryStream = new MemoryStream();
        var buffer = new byte[512];
        Debug.WriteLine("DataAvailable:{0}", networkStream.DataAvailable);
        do
        {
            var bytesRead = networkStream.Read(buffer);
            memoryStream.Write(buffer, 0, bytesRead);
            Debug.WriteLine($"Loop Write {bytesRead} bytes, Position:{memoryStream.Position}");
        } while (networkStream.DataAvailable);

        Debug.WriteLine($"Total Write: {memoryStream.Length},Position:{memoryStream.Position}");
        memoryStream.Position = 0;
        return memoryStream;
    }
}