namespace JustProxies.RuleEngine.Core.Models;

public class RuleItems : List<RuleItem>
{
    public RuleCondition Meet { get; set; } = RuleCondition.All;
}