namespace JustProxies.Proxy.Setup.Core;

public interface IWebProxyToolImpl
{
    WebProxyInfo GetWebProxy();
    bool SetWebProxy(string address, int port);
    bool DisableProxy();
    bool EnableProxy();
}