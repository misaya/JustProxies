namespace JustProxies.RuleEngine.Core.Models;

public enum RuleMatch
{
    /// <summary>
    /// 满足所有
    /// </summary>
    All,

    /// <summary>
    /// 满足任意条件
    /// </summary>
    Any,

    /// <summary>
    /// 不满足任意条件
    /// </summary>
    None
}