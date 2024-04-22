namespace JustProxies.RuleEngine.Core.Models;

public record RuleItemMatchResult(RuleItem Item, bool IsMatched)
{
    public RuleItem Item { get; internal set; } = Item;
    public bool IsMatched { get; internal set; } = IsMatched;
}