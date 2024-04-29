namespace JustProxies.RuleEngine.Core.Models;

public class RuleActionResource : Dictionary<string, string>
{
    public string Url
    {
        get => this["url"];
        set => this["url"] = value;
    }

    public string Body
    {
        get => this["body"];
        set => this["body"] = value;
    }
}