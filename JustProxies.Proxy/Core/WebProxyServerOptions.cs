using System.Net;

namespace JustProxies.Proxy.Core;

public class WebProxyServerOptions
{
    public const string OptionsName = nameof(WebProxyServerOptions);
    public string NetworkInterface { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Port { get; set; } = 0;

    public IPAddress GetIPAddress()
    {
        return IPAddress.TryParse(Address, out var ipAddress) ? ipAddress : IPAddress.None;
    }

    public void ThrowExceptionIfInvalid()
    {
        if (!IPEndPoint.TryParse($"{this.Address}:{this.Port}", out _))
        {
            throw new ArgumentException($"配置错误");
        }
    }
}