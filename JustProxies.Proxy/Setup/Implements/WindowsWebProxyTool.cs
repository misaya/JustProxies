using JustProxies.Proxy.Setup.Core;

namespace JustProxies.Proxy.Setup.Implements;

public class WindowsWebProxyTool(string network) : IWebProxyToolImpl
{
    private readonly string _network = network;

    public WebProxyInfo GetWebProxy()
    {
        throw new NotImplementedException();
    }

    public bool SetWebProxy(string address, int port)
    {
        throw new NotImplementedException();
    }

    public bool DisableProxy()
    {
        throw new NotImplementedException();
    }

    public bool EnableProxy()
    {
        throw new NotImplementedException();
    }
}