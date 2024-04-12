using System.Diagnostics;

namespace JustProxies.Proxy.Setup.Core;

[DebuggerDisplay("网卡：{NetworkService} 代理服务器：{Host} 端口：{Port} 启用状态：{IsEnabled}")]
public class WebProxyInfo
{
    public string Host { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string NetworkService { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }

    public string ToString(string format = "网卡：{0} 代理服务器：{1} 端口：{2} 启用状态：{3}")
    {
        return string.Format(format, NetworkService, Host, Port, IsEnabled);
    }
}