namespace JustProxies.RuleEngine.Core.Models;

public class RuleItem
{
    public RuleItemTarget Target { get; set; } = RuleItemTarget.Body;
    public RuleItemMethod Method { get; set; } = RuleItemMethod.Contains;
    public string RawData { get; set; } = string.Empty;
}