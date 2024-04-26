using JustProxies.RuleEngine.Core;
using JustProxies.RuleEngine.Core.Models;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace JustProxies.RuleEngine;

public class RuleManagement : IRuleManagement, IDisposable
{
    private readonly ILogger<RuleManagement> _logger;
    private readonly DataBase _dataBase = DataBase.Instance;

    public RuleManagement(ILogger<RuleManagement> logger)
    {
        _logger = logger;
    }

    public void Add(RulePackage package)
    {
        var col = _dataBase.GetCollection<RulePackage>();
        col.EnsureIndex(x => x.RuleId, true);
        var result = col.Insert(package);
        _logger.LogInformation("添加规则{package}结果{result}", package, result);
    }

    public bool Delete(Guid ruleId)
    {
        var col = _dataBase.GetCollection<RulePackage>();
        var result = col.DeleteMany(p => p.RuleId == ruleId);
        return result == 1;
    }

    public bool DeleteAll()
    {
        var col = _dataBase.GetCollection<RulePackage>();
        col.DeleteAll();
        return true;
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