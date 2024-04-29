using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using JustProxies.Proxy.Exts;
using NetworkStream = System.Net.Sockets.NetworkStream;

namespace JustProxies.Proxy.Core.Internal;

[DebuggerDisplay("{HttpMethod} {RawUrl}")]
public class HttpRequest
{
    #region privates

    private int Position { get; set; }
    private Version Version { get; set; } = null!;

    private void Init(MemoryStream memoryStream)
    {
        TotalContent = memoryStream.ToArray();
        using var reader = new StreamReader(memoryStream);
        var requestLine = reader.ReadLine();
        Debug.WriteLine(requestLine);
        var requestLineSplit = requestLine?.Split(' ');
        HttpMethod = new HttpMethod(requestLineSplit!.First());
        RawUrl = requestLineSplit![1];
        Version = new Version(requestLineSplit[2].Split('/')[1]);
        if (HttpMethod == HttpMethod.Connect)
            Url = RawUrl.EndsWith("443")
                ? new Uri("https" + "://" + RawUrl)
                : new Uri("http" + "://" + RawUrl);
        else
            Url = new Uri(RawUrl);
        while (reader.ReadLine() is { } headerLine &&
               !string.IsNullOrWhiteSpace(headerLine))
        {
            Headers.Add(headerLine);
        }

        Position = (int)memoryStream.Position;
        Body = reader.ReadToEnd();
    }

    #endregion


    internal HttpRequest(NetworkStream requestStream)
    {
        using var memoryStream = requestStream.ReadRequest();
        Init(memoryStream);
    }

    public HttpMethod HttpMethod { get; private set; } = null!;
    public HttpRequestHeaders Headers { get; } = [];
    public string RawUrl { get; private set; } = null!;
    public Uri Url { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public ReadOnlyMemory<byte> TotalContent { get; private set; }

    public void UrlReWrite(Uri newUrl)
    {
        var headers = this.Headers
            .Where(p => !p.Key.Equals("host", StringComparison.CurrentCultureIgnoreCase))
            .Select(p => $"{p.Key}: {string.Join(";", p.Value)}").ToList();
        headers.Add($"Host: {newUrl.Host}");
        headers.Add($"Proxy: JustProxies");
        var firstLine = $"{this.HttpMethod} {newUrl.PathAndQuery} HTTP/{this.Version}\n";
        var headerLine = string.Join("\n", headers);
        var request = Encoding.ASCII.GetBytes($"{firstLine}{headerLine}\n\n").ToList();
        request.AddRange(TotalContent.ToArray().Skip(Position).ToArray());
        using var memoryStream = new MemoryStream(request.ToArray());
        Init(memoryStream);
        this.Url = newUrl;
        this.RawUrl = newUrl.PathAndQuery;
    }
}