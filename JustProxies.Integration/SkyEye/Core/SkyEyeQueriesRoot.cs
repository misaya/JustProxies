using System.Collections.ObjectModel;

namespace JustProxies.Integration.SkyEye;

public abstract class SkyEyeQueriesRoot
{
    private readonly Dictionary<string, string> _appList = new();

    internal void AddApp(string appId, string appToken)
    {
        this._appList.Add(appId, appToken);
    }

    /// <summary>
    /// 项目id
    /// </summary>
    public ReadOnlyCollection<string> AppIds => new ReadOnlyCollection<string>(_appList.Keys.ToList());

    /// <summary>
    /// 如果不使用Header传token，可以把token字符串放到数组中
    /// </summary>
    public ReadOnlyCollection<string> Tokens => new ReadOnlyCollection<string>(_appList.Values.ToList());
}