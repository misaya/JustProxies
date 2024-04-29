using JustProxies.Proxy.Core.Internal;

namespace JustProxies.RuleEngine.Core.Models;

public class RulePackage
{
    public Guid RuleId { get; set; } = new();
    public string RuleName { get; set; } = string.Empty;
    public string RuleDescription { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public RuleItems Rules { get; set; } = new();
    public RuleAction Action { get; set; } = new();

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }

    public bool IsMatch(HttpContext context)
    {
        var results = new List<RuleItemMatchResult>();
        foreach (var condition in this.Rules)
        {
            var isMatch = condition.IsMatch(context);
            results.Add(new RuleItemMatchResult(condition, isMatch));
            if (isMatch && this.Rules.Match == RuleMatch.Any)
            {
                return true;
            }
        }

        return this.Rules.Match switch
        {
            RuleMatch.Any => results.Any(p => p.IsMatched),
            RuleMatch.All => results.All(p => p.IsMatched),
            RuleMatch.None => results.All(p => !p.IsMatched),
        };
    }
}