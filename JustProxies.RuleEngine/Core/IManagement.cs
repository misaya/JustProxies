using JustProxies.RuleEngine.Core.Models;

namespace JustProxies.RuleEngine.Core;

public interface IManagement
{
    bool AddRule(RuleData ruleData);

    bool DeleteRule(Guid ruleId);

    bool AddRuleCondition(Guid ruleId, RuleCondition condition);

    List<RuleData> GetEnabledRules();
}