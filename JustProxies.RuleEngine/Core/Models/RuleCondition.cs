namespace JustProxies.RuleEngine.Core.Models;

public class RuleCondition
{
    public RuleConditionTarget Target { get; set; } = RuleConditionTarget.Body;
    public RuleConditionMethod Method { get; set; } = RuleConditionMethod.Contains;
    public string RawData { get; set; } = string.Empty;
}