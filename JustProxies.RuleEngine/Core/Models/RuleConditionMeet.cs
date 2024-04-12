namespace JustProxies.RuleEngine.Core.Models;

public enum RuleConditionMeet
{
    /// <summary>
    ///     满足所有
    /// </summary>
    All,

    /// <summary>
    ///     满足任一条件
    /// </summary>
    Any,

    /// <summary>
    ///     不满足任一条件
    /// </summary>
    None
}