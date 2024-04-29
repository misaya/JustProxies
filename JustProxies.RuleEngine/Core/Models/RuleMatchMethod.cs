namespace JustProxies.RuleEngine.Core.Models;

public enum RuleMatchMethod
{
    Equal,
    NotEqual,
    Contains,
    NotContains,
    StartsWith,
    NotStartsWith,
    EndWith,
    NotEndWith,
    Empty,
    NotEmpty,
    RegularExpression,
    RuleScript
}