using System.Net;

namespace JustProxies.Proxy.Core.Options;

public class HttpProxyServerOptions
{
    public const string OptionsName = nameof(HttpProxyServerOptions);
    public string NetworkInterface { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public int Port { get; init; } = 0;
    public bool EnableBuffer { get; init; } = true;

    public IPAddress GetIPAddress()
    {
        return IPAddress.TryParse(Address, out var ipAddress) ? ipAddress : IPAddress.None;
    }

    public void ThrowExceptionIfInvalid()
    {
        if (!IPEndPoint.TryParse($"{Address}:{Port}", out _)) throw new ArgumentException("配置错误");
    }
}