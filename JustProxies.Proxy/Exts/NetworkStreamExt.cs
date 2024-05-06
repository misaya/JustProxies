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
        var statusLine = networkStream.ReadLine()!;
        memoryStream.Write(statusLine);
        if (!Encoding.ASCII.GetString(statusLine).StartsWith("HTTP/1.1 200"))
        {
            memoryStream.Position = 0;
            return memoryStream;
        }

        var isChunked = false;
        while (networkStream.ReadLine() is { } header)
        {
            memoryStream.Write(header);
            var text = Encoding.ASCII.GetString(header);
            if (text.StartsWith("Transfer-Encoding: chunked"))
            {
                isChunked = true;
            }

            if (text.Trim().Length == 0)
            {
                break;
            }
        }

        if (isChunked)
        {
            while (true)
            {
                var chunkSizeBytes = networkStream.ReadLine();
                memoryStream.Write(chunkSizeBytes);
                var chunkSizeHex = Encoding.ASCII.GetString(chunkSizeBytes);
                if (chunkSizeHex.Trim().Length == 0)
                {
                   continue;
                }

                if (int.TryParse(chunkSizeHex, NumberStyles.HexNumber, null,
                        out var chunkSize))
                {
                    if (chunkSize == 0)
                    {
                        break;
                    }

                    Debug.WriteLine($"Chunk Size:{chunkSize}");
                    while (chunkSize > 0)
                    {
                        var buffer = new byte[chunkSize];
                        var length = networkStream.Read(buffer, 0, buffer.Length);
                        memoryStream.Write(buffer, 0, length);
                        Debug.WriteLine($"----> Read length:{length}");
                        chunkSize -= length;
                    }
                }
                else
                {
                    Debug.WriteLine($"Error - {chunkSizeHex}");
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

    public static Byte[] ReadLine(this NetworkStream stream)
    {
        var total = new List<byte>();
        var buffer = new byte[1];

        while (true)
        {
            while (true)
            {
                var length = stream.Read(buffer, 0, 1);
                if (length == 1)
                {
                    break;
                }
            }

            total.Add(buffer[0]);
            if (buffer[0] == '\n')
            {
                return total.ToArray();
            }
        }
    }
}