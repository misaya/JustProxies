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
}