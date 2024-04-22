using System.Text.RegularExpressions;
using JustProxies.Proxy.Core.Internal;
using JustProxies.RuleEngine.Core.Models;

namespace JustProxies.RuleEngine.Exts;

public static class RuleItemExt
{
    private static bool IsMatch(this RuleItem ruleItem, HttpContext context)
    {
        if (ruleItem.Target == RuleItemTarget.Header)
        {
            return ruleItem.Method switch
            {
                RuleItemMethod.Empty => !context.Request.Headers.HasKeys(),
                RuleItemMethod.NotEmpty => context.Request.Headers.HasKeys(),
                RuleItemMethod.Contains => context.Request.Headers.IsExist(ruleItem.RawData),
                RuleItemMethod.NotContains => !context.Request.Headers.IsExist(ruleItem.RawData),
                _ => false
            };
        }

        var value = ruleItem.Target switch
        {
            RuleItemTarget.RemoteIP => context.RemoteEndPoint.ToString(),
            RuleItemTarget.Method => context.Request.HttpMethod.ToString(),
            RuleItemTarget.Url => context.Request.Url == null ? "" : context.Request.Url!.ToString(),
            RuleItemTarget.Body => context.Request.Body,
            _ => null!
        };
        switch (ruleItem.Method)
        {
            case RuleItemMethod.Equal:
                return value.Equals(ruleItem.RawData, StringComparison.OrdinalIgnoreCase);
            case RuleItemMethod.NotEqual:
                return !value.Equals(ruleItem.RawData, StringComparison.OrdinalIgnoreCase);
            case RuleItemMethod.Contains:
                return value.Contains(ruleItem.RawData);
            case RuleItemMethod.NotContains:
                return !value.Contains(ruleItem.RawData);
            case RuleItemMethod.StartsWith:
                return value.StartsWith(ruleItem.RawData);
            case RuleItemMethod.NotStartsWith:
                return !value.StartsWith(ruleItem.RawData);
            case RuleItemMethod.EndWith:
                return value.EndsWith(ruleItem.RawData);
            case RuleItemMethod.NotEndWith:
                return !value.EndsWith(ruleItem.RawData);
            case RuleItemMethod.Empty:
                return string.IsNullOrEmpty(value);
            case RuleItemMethod.NotEmpty:
                return !string.IsNullOrEmpty(value);
            case RuleItemMethod.RegularExpression:
                return Regex.IsMatch(value, ruleItem.RawData);
            case RuleItemMethod.RuleScript:
                var script = new RuleItemScript(ruleItem.RawData);
                return script.Compile().Invoke(context);
            default:
                return false;
        }
    }

    public static bool IsMatch(this RulePackage rulePackage, HttpContext context)
    {
        var results = new List<RuleItemMatchResult>();
        foreach (var condition in rulePackage.Rules)
        {
            var isMatch = condition.IsMatch(context);
            results.Add(new RuleItemMatchResult(condition, isMatch));
            if (isMatch && rulePackage.Rules.Meet == RuleCondition.Any)
            {
                return true;
            }
        }

        return rulePackage.Rules.Meet switch
        {
            RuleCondition.Any => results.Any(p => p.IsMatched),
            RuleCondition.All => results.All(p => p.IsMatched),
            RuleCondition.None => results.All(p => !p.IsMatched),
        };
    }
}