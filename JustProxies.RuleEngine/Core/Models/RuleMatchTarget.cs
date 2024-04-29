namespace JustProxies.RuleEngine.Core.Models;

public enum RuleMatchTarget
{
    /// <summary>
    /// 请求来源IP地址
    /// </summary>
    RemoteIP,

    /// <summary>
    /// 请求方式GET,POST...
    /// </summary>
    Method,

    /// <summary>
    /// 请求头
    /// </summary>
    Header,
    
    /// <summary>
    /// 请求URL
    /// </summary>
    Url,

    /// <summary>
    /// 请求报文
    /// </summary>
    Body
}