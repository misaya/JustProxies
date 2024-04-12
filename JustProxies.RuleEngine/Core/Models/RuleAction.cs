namespace JustProxies.RuleEngine.Core.Models;

public class RuleAction
{
    public string ActionName { get; set; } = string.Empty;
    public string ActionDescription { get; set; } = string.Empty;
    public RuleActionType ActionType { get; set; } = RuleActionType.Default;
    public string Resource { get; set; } = string.Empty;
}