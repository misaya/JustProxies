using JustProxies.RuleEngine.Core;
using JustProxies.RuleEngine.Core.Models;

namespace JustProxies.RuleEngine;

public class Management : IManagement
{
   public bool AddRule(RuleData ruleData)
   {
      throw new NotImplementedException();
   }

   public bool DeleteRule(Guid ruleId)
   {
      throw new NotImplementedException();
   }

   public bool AddRuleCondition(Guid ruleId, RuleCondition condition)
   {
      throw new NotImplementedException();
   }

   public List<RuleData> GetEnabledRules()
   {
      throw new NotImplementedException();
   }
}