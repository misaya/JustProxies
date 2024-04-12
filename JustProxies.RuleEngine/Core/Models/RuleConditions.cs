namespace JustProxies.RuleEngine.Core.Models;

public class RuleConditions : List<RuleCondition>
{
    public RuleConditionMeet Meet { get; set; } = RuleConditionMeet.All;
}