using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace JustProxies.Proxy.Core;

[DebuggerDisplay("来自{RemoteEndPoint}的{HttpMethod}请求{RawUrl}")]
public class HttpRequest
{
    internal HttpRequest(TcpClient client, List<byte> totalBytes)
    {
        try
        {
            this.RawData = totalBytes;
            using var stream = new MemoryStream(totalBytes.ToArray());
            using var reader = new StreamReader(stream);
            var requestLine = reader.ReadLine();
            var requestLineSplit = requestLine?.Split(' ');
            if (requestLineSplit?.FirstOrDefault() is { } method)
            {
                this.HttpMethod = new HttpMethod(method);
                this.RawUrl = requestLineSplit[1];
                this.Version = requestLineSplit[2];

                if (HttpMethod == HttpMethod.Connect)
                {
                    this.Url = this.RawUrl.EndsWith("443")
                        ? new Uri("https://" + this.RawUrl)
                        : new Uri("http://" + this.RawUrl);
                }
                else
                {
                    this.Url = new Uri(this.RawUrl);
                }
            }

            while (reader.ReadLine() is { } headerLine && !string.IsNullOrWhiteSpace(headerLine))
            {
                this.Headers.Add(headerLine);
            }

            this.Body = reader.ReadToEnd();
            this.RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint ?? null!;
        }
        catch (Exception exception)
        {
            throw new HttpRequestException("Invalid request", exception, HttpStatusCode.BadRequest);
        }
    }

    public string Version { get; private set; } = null!;
    public string RawUrl { get; private set; } = null!;
    public Uri Url { get; private set; } = null!;
    public string Body { get; private set; }
    public HttpMethod HttpMethod { get; private set; } = null!;
    public HttpRequestHeaders Headers { get; private set; } = [];
    public IPEndPoint RemoteEndPoint { get; private set; } = null!;
    public List<byte> RawData { get; private set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{this.HttpMethod} {this.RawUrl} {this.Version}");
        sb.AppendLine($"{this.Headers}");
        sb.AppendLine();
        sb.AppendLine(this.Body);
        return sb.ToString();
    }
}