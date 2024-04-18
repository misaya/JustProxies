using System.Diagnostics;
using System.Net.Sockets;
using JustProxies.Proxy.Exts;

namespace JustProxies.Proxy.Core;

[DebuggerDisplay("{HttpMethod} {RawUrl}")]
public class HttpRequest
{
    internal HttpRequest(NetworkStream requestStream)
    {
        using var memoryStream = requestStream.ReadToMemoryStream();
        this.RequestRawData = memoryStream.ToArray();
        using var reader = new StreamReader(memoryStream);
        var requestLine = reader.ReadLine();
        Debug.WriteLine(requestLine);
        var requestLineSplit = requestLine?.Split(' ');
        this.HttpMethod = new HttpMethod(requestLineSplit!.First());
        this.RawUrl = requestLineSplit![1];
        this.Version = new Version(requestLineSplit[2].Split('/')[1]);
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

        while (reader.ReadLine() is { } headerLine &&
               !string.IsNullOrWhiteSpace(headerLine))
        {
            this.Headers.Add(headerLine);
            Debug.WriteLine(headerLine);
        }

        this.Body = reader.ReadToEnd();
    }

    public ReadOnlyMemory<byte> RequestRawData { get; }
    public Version Version { get; }
    public string RawUrl { get; }
    public Uri Url { get; }
    public string Body { get; }
    public HttpMethod HttpMethod { get; }
    public HttpRequestHeaders Headers { get; } = [];

    public override string ToString()
    {
        return $"{this.HttpMethod} {this.RawUrl} {this.Version}";
    }
}