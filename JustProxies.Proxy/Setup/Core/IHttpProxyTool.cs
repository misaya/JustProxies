namespace JustProxies.Proxy.Setup.Core;

public interface IHttpProxyTool : IWebProxyToolImpl
{
    public bool SetWebProxy();
}