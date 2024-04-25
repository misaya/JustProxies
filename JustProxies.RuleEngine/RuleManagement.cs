using JustProxies.RuleEngine.Core;
using JustProxies.RuleEngine.Core.Models;
using LiteDB;

namespace JustProxies.RuleEngine;

public class RuleManagement : IRuleManagement, IDisposable
{
    private readonly DataBase _dataBase = DataBase.Instance;

    public void Add(RulePackage package)
    {
        var col = _dataBase.GetCollection<RulePackage>();
        col.EnsureIndex(x => x.RuleId, true);
        col.Insert(package);
    }

    public bool Delete(Guid ruleId)
    {
        var col = _dataBase.GetCollection<RulePackage>();
        var result = col.DeleteMany(p => p.RuleId == ruleId);
        return result == 1;
    }

    public List<RulePackage> GetEnabled()
    {
        var col = _dataBase.GetCollection<RulePackage>();
        var result = col.Find(p => p.IsEnabled == true);
        return result.ToList();
    }

    public void Dispose()
    {
        _dataBase.Dispose();
    }
}