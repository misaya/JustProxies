namespace JustProxies.RuleEngine.Core.Models;

public class RuleAction
{
    public RuleActionType ActionType { get; set; } = RuleActionType.ByPass;
    public RuleActionResource Resource { get; set; } = new();
}