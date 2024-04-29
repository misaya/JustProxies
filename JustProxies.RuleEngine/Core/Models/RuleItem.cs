using System.Text.RegularExpressions;
using JustProxies.Proxy.Core.Internal;

namespace JustProxies.RuleEngine.Core.Models;

public class RuleItem
{
    public RuleMatchTarget Target { get; set; } = RuleMatchTarget.Body;
    public RuleMatchMethod Method { get; set; } = RuleMatchMethod.Contains;
    public string RawData { get; set; } = string.Empty;

    public bool IsMatch(HttpContext context)
    {
        if (this.Target == RuleMatchTarget.Header)
        {
            return this.Method switch
            {
                RuleMatchMethod.Empty => !context.Request.Headers.HasKeys(),
                RuleMatchMethod.NotEmpty => context.Request.Headers.HasKeys(),
                RuleMatchMethod.Contains => context.Request.Headers.IsExist(this.RawData),
                RuleMatchMethod.NotContains => !context.Request.Headers.IsExist(this.RawData),
                _ => false
            };
        }

        var value = this.Target switch
        {
            RuleMatchTarget.RemoteIP => context.RemoteEndPoint.ToString(),
            RuleMatchTarget.Method => context.Request.HttpMethod.ToString(),
            RuleMatchTarget.Url => context.Request.Url == null ? "" : context.Request.Url!.ToString(),
            RuleMatchTarget.Body => context.Request.Body,
            _ => null!
        };
        switch (this.Method)
        {
            case RuleMatchMethod.Equal:
                return value.Equals(this.RawData, StringComparison.OrdinalIgnoreCase);
            case RuleMatchMethod.NotEqual:
                return !value.Equals(this.RawData, StringComparison.OrdinalIgnoreCase);
            case RuleMatchMethod.Contains:
                return value.Contains(this.RawData);
            case RuleMatchMethod.NotContains:
                return !value.Contains(this.RawData);
            case RuleMatchMethod.StartsWith:
                return value.StartsWith(this.RawData);
            case RuleMatchMethod.NotStartsWith:
                return !value.StartsWith(this.RawData);
            case RuleMatchMethod.EndWith:
                return value.EndsWith(this.RawData);
            case RuleMatchMethod.NotEndWith:
                return !value.EndsWith(this.RawData);
            case RuleMatchMethod.Empty:
                return string.IsNullOrEmpty(value);
            case RuleMatchMethod.NotEmpty:
                return !string.IsNullOrEmpty(value);
            case RuleMatchMethod.RegularExpression:
                return Regex.IsMatch(value, this.RawData);
            case RuleMatchMethod.RuleScript:
                var script = new RuleMatchScript(this.RawData);
                return script.Compile().Invoke(context);
            default:
                return false;
        }
    }
}