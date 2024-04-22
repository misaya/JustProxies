using JustProxies.Proxy.Core.Options;
using JustProxies.Proxy.Setup.Core;
using JustProxies.Proxy.Setup.Implements;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JustProxies.Proxy.Setup;

public class WebProxyTool : IWebProxyTool
{
    private readonly ILogger<WebProxyTool> _logger;
    private readonly IOptions<HttpProxyServerOptions> _options;
    private readonly IWebProxyToolImpl _tool;

    public WebProxyTool(ILogger<WebProxyTool> logger, IOptions<HttpProxyServerOptions> options)
    {
        _logger = logger;
        _options = options;
        _tool = GetWebProxyTool();
    }

    public WebProxyInfo GetWebProxy()
    {
        var result = _tool.GetWebProxy();
        _logger.LogInformation("获取代理信息：{@result}", result.ToString());
        return result;
    }

    public bool SetWebProxy()
    {
        return SetWebProxy(_options.Value.Address, _options.Value.Port);
    }

    public bool SetWebProxy(string address, int port)
    {
        var result = _tool.SetWebProxy(address, port);
        _logger.Log(result ? LogLevel.Information : LogLevel.Warning, "设置代理信息：地址:{@address} 端口:{@port} 结果:{@result}",
            address,
            port, result ? "成功" : "失败");
        return result;
    }

    public bool DisableProxy()
    {
        var result = _tool.DisableProxy();
        _logger.Log(result ? LogLevel.Information : LogLevel.Warning, "禁用代理信息：{@result}", result ? "成功" : "失败");
        return result;
    }

    public bool EnableProxy()
    {
        var result = _tool.EnableProxy();
        _logger.Log(result ? LogLevel.Information : LogLevel.Warning, "启用代理信息：{@result}", result ? "成功" : "失败");
        return result;
    }

    private IWebProxyToolImpl GetWebProxyTool()
    {
        var network = _options.Value.NetworkInterface;
        return Environment.OSVersion.Platform switch
        {
            PlatformID.Unix => new MacOsWebProxyTool(network),
            PlatformID.MacOSX => new MacOsWebProxyTool(network),
            PlatformID.Win32NT => new WindowsWebProxyTool(network),
            _ => throw new PlatformNotSupportedException($"当前平台 {Environment.OSVersion.Platform} 未支持")
        };
    }
}