using System.Diagnostics;
using System.Net.Sockets;
using JustProxies.Proxy.Exts;

namespace JustProxies.Proxy.Core.Internal;

[DebuggerDisplay("{HttpMethod} {RawUrl}")]
public class HttpRequest
{
    internal HttpRequest(NetworkStream requestStream)
    {
        using var memoryStream = requestStream.ReadAll();
        RequestRawData = memoryStream.ToArray();
        using var reader = new StreamReader(memoryStream);
        var requestLine = reader.ReadLine();
        Debug.WriteLine(requestLine);
        var requestLineSplit = requestLine?.Split(' ');
        HttpMethod = new HttpMethod(requestLineSplit!.First());
        RawUrl = requestLineSplit![1];
        Version = new Version(requestLineSplit[2].Split('/')[1]);
        if (HttpMethod == HttpMethod.Connect)
            Url = RawUrl.EndsWith("443")
                ? new Uri("https://" + RawUrl)
                : new Uri("http://" + RawUrl);
        else
            Url = new Uri(RawUrl);

        while (reader.ReadLine() is { } headerLine &&
               !string.IsNullOrWhiteSpace(headerLine))
        {
            Headers.Add(headerLine);
            Debug.WriteLine(headerLine);
        }

        Position = (int)memoryStream.Position;
        Body = reader.ReadToEnd();
    }

    private int Position { get; }
    public ReadOnlyMemory<byte> RequestRawData { get; }
    public Version Version { get; }
    public string RawUrl { get; private set; }
    public Uri Url { get; private set; }
    public string Body { get; }
    public HttpMethod HttpMethod { get; }
    public HttpRequestHeaders Headers { get; } = [];

    public ActivityTraceId TraceId { get; } = ActivityTraceId.CreateRandom();

    public override string ToString()
    {
        return $"{HttpMethod} {RawUrl} HTTP/{Version}";
    }

    public HttpRequestMessage GetHttpRequestMessage()
    {
        var message = new HttpRequestMessage(this.HttpMethod, this.Url);
        foreach (var header in this.Headers)
        {
            message.Headers.Add(header.Key, header.Value);
        }

        var data = this.RequestRawData.ToArray().Skip(this.Position).ToArray();
        using var stream = new MemoryStream(data);
        message.Content = new StreamContent(stream);
        message.Version = this.Version;
        return message;
    }
}