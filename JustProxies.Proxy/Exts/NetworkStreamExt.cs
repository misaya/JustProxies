using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace JustProxies.Proxy.Exts;

public static class NetworkStreamExt
{
    public static MemoryStream ReadRequest(this NetworkStream networkStream)
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

    public static MemoryStream ReadResponse(this NetworkStream networkStream)
    {
        var memoryStream = new MemoryStream();

        // 读取HTTP响应
        using var reader = new StreamReader(networkStream, Encoding.UTF8);
        var statusLine = reader.ReadLine()!;
        memoryStream.Write(Encoding.ASCII.GetBytes(statusLine));
        memoryStream.Write("\n"u8);
        if (!statusLine.StartsWith("HTTP/1.1 200"))
        {
            memoryStream.Position = 0;
            return memoryStream;
        }

        var isChunked = false;
        while (reader.ReadLine() is { } header && header.Length > 0)
        {
            if (header.StartsWith("Transfer-Encoding: chunked"))
            {
                isChunked = true;
            }

            memoryStream.Write(Encoding.ASCII.GetBytes(header));
            memoryStream.Write("\n"u8);
        }

        memoryStream.Write("\n"u8);

        if (isChunked)
        {
            while (true)
            {
                var chunkSizeLine = reader.ReadLine()!;
                memoryStream.Write(Encoding.ASCII.GetBytes(chunkSizeLine));
                memoryStream.Write("\r\n"u8);
                if (int.TryParse(chunkSizeLine, NumberStyles.HexNumber, null, out var chunkSize))
                {
                    Debug.WriteLine($"Start Read chunkSize:{chunkSize}");
                    while (chunkSize > 0)
                    {
                        var buffer = new byte[chunkSize];
                        var length = networkStream.Read(buffer, 0, buffer.Length);
                        memoryStream.Write(buffer, 0, length);
                        if (length != chunkSize)
                        {
                            Debug.WriteLine($"Loop - length:{length}, chunkSize:{chunkSize}");
                        }

                        chunkSize -= length;
                    }
                }
                else
                {
                    Debug.WriteLine($"Error - {chunkSizeLine}");
                }
            }
        }
        else
        {
            var buffer = new byte[512];
            do
            {
                var bytesRead = networkStream.Read(buffer);
                memoryStream.Write(buffer, 0, bytesRead);
            } while (networkStream.DataAvailable);
        }


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