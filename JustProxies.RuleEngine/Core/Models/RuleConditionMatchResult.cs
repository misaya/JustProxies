namespace JustProxies.RuleEngine.Core.Models;

public class RuleConditionMatchResult(RuleCondition condition)
{
    public RuleCondition Condition { get; internal set; } = condition;
    public bool IsMatched { get; internal set; } = false;
}