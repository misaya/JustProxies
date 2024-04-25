using JustProxies.RuleEngine.Core.Models;

namespace JustProxies.RuleEngine.Core;

public interface IRuleManagement
{
    void Add(RulePackage package);

    bool Delete(Guid ruleId);

    List<RulePackage> GetEnabled();
}