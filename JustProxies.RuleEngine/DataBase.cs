using LiteDB;

namespace JustProxies.RuleEngine;

public class DataBase : IDisposable
{
    private const string DbName = "JustProxies.db";
    private readonly LiteDatabase _db = new(DbName);

    public void Dispose()
    {
        _db.Dispose();
    }

    public ILiteCollection<T> GetCollection<T>()
    {
        return _db.GetCollection<T>(typeof(T).Name);
    }

    public static DataBase Instance => new();
}