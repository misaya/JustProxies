using JustProxies.RuleEngine.Core.Models;

namespace JustProxies.RuleEngine.Core;

public interface IManagement
{
    void Add(RulePackage package);

    bool Delete(Guid ruleId);

    List<RulePackage> GetEnabled();
}