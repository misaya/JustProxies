namespace JustProxies.RuleEngine.Core.Models;

public class RuleItems : List<RuleItem>
{
    public RuleMatch Match { get; set; } = RuleMatch.All;
}