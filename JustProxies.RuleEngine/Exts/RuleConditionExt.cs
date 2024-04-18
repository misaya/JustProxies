using System.Net;
using System.Text.RegularExpressions;
using JustProxies.Proxy.Core;
using JustProxies.RuleEngine.Core.Models;

namespace JustProxies.RuleEngine.Exts;

public static class RuleConditionExt
{
    private static bool IsMatch(this RuleCondition ruleCondition, HttpContext context)
    {
        if (ruleCondition.Target == RuleConditionTarget.Header)
        {
            return ruleCondition.Method switch
            {
                RuleConditionMethod.Empty => !context.Request.Headers.HasKeys(),
                RuleConditionMethod.NotEmpty => context.Request.Headers.HasKeys(),
                RuleConditionMethod.Contains => context.Request.Headers.IsExist(ruleCondition.RawData),
                RuleConditionMethod.NotContains => !context.Request.Headers.IsExist(ruleCondition.RawData),
                _ => false
            };
        }
        
        var value = ruleCondition.Target switch
        {
            RuleConditionTarget.RemoteIP => context.RemoteEndPoint.ToString(),
            RuleConditionTarget.Method => context.Request.HttpMethod.ToString(),
            RuleConditionTarget.Url => context.Request.Url == null ? "" : context.Request.Url!.ToString(),
            RuleConditionTarget.Body => context.Request.Body,
            _ => null!
        };
        switch (ruleCondition.Method)
        {
            case RuleConditionMethod.Equal:
                return value.Equals(ruleCondition.RawData, StringComparison.OrdinalIgnoreCase);
            case RuleConditionMethod.NotEqual:
                return !value.Equals(ruleCondition.RawData, StringComparison.OrdinalIgnoreCase);
            case RuleConditionMethod.Contains:
                return value.Contains(ruleCondition.RawData);
            case RuleConditionMethod.NotContains:
                return !value.Contains(ruleCondition.RawData);
            case RuleConditionMethod.StartsWith:
                return value.StartsWith(ruleCondition.RawData);
            case RuleConditionMethod.NotStartsWith:
                return !value.StartsWith(ruleCondition.RawData);
            case RuleConditionMethod.EndWith:
                return value.EndsWith(ruleCondition.RawData);
            case RuleConditionMethod.NotEndWith:
                return !value.EndsWith(ruleCondition.RawData);
            case RuleConditionMethod.Empty:
                return string.IsNullOrEmpty(value);
            case RuleConditionMethod.NotEmpty:
                return !string.IsNullOrEmpty(value);
            case RuleConditionMethod.RegularExpression:
                return Regex.IsMatch(value, ruleCondition.RawData);
            case RuleConditionMethod.RuleScript:
                var script = new RuleScript(ruleCondition.RawData);
                return script.Compile().Invoke(context);
            default:
                return false;
        }
    }

    public static bool IsMatch(this RuleData ruleData, HttpContext context)
    {
        var results = new List<RuleConditionMatchResult>();
        foreach (var condition in ruleData.Conditions)
        {
            var isMatch = condition.IsMatch(context);
            results.Add(new RuleConditionMatchResult(condition)
            {
                IsMatched = isMatch
            });
            if (isMatch && ruleData.Conditions.Meet == RuleConditionMeet.Any)
            {
                return true;
            }
        }

        return ruleData.Conditions.Meet switch
        {
            RuleConditionMeet.Any => results.Any(p => p.IsMatched),
            RuleConditionMeet.All => results.All(p => p.IsMatched),
            RuleConditionMeet.None => results.All(p => !p.IsMatched),
        };
    }
}