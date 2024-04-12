using System.Net;
using JustProxies.Proxy.Core;

namespace JustProxies.RuleEngine.Core.Models;

public class RuleData
{
    public Guid RuleId { get; set; } = new();
    public string RuleName { get; set; } = string.Empty;
    public string RuleDescription { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public RuleConditions Conditions { get; set; } = new();
    public RuleAction Action { get; set; } = new();

    public Task<bool> Invoke(HttpContext httpContext)
    {
        throw new NotImplementedException();
    }
}