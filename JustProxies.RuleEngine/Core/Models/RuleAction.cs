namespace JustProxies.RuleEngine.Core.Models;

public class RuleAction
{
    public RuleActionType ActionType { get; set; } = RuleActionType.Default;
    public string Resource { get; set; } = string.Empty;
}