namespace JustProxies.RuleEngine.Core.Models;

public enum RuleActionType
{
    /// <summary>
    /// 代理请求原请求
    /// </summary>
    ByPass,

    /// <summary>
    /// 换URL发起请求，并返回请求内容
    /// </summary>
    UrlReWrite,

    /// <summary>
    /// 返回自定义内容
    /// </summary>
    CustomizeResponseContent,
}