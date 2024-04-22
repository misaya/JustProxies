namespace JustProxies.RuleEngine.Core.Models;

public enum RuleItemMethod
{
    /// <summary>
    ///     等于
    /// </summary>
    Equal,

    /// <summary>
    ///     不等于
    /// </summary>
    NotEqual,

    /// <summary>
    ///     包含
    /// </summary>
    Contains,

    /// <summary>
    ///     不包含
    /// </summary>
    NotContains,

    /// <summary>
    ///     开头是
    /// </summary>
    StartsWith,

    /// <summary>
    ///     开头不是
    /// </summary>
    NotStartsWith,

    /// <summary>
    ///     结束于
    /// </summary>
    EndWith,

    /// <summary>
    ///     不结束于
    /// </summary>
    NotEndWith,

    /// <summary>
    ///     为空
    /// </summary>
    Empty,

    /// <summary>
    ///     不为空
    /// </summary>
    NotEmpty,

    /// <summary>
    /// 正则表达式
    /// </summary>
    RegularExpression,

    /// <summary>
    /// 规则脚本
    /// </summary>
    RuleScript
}