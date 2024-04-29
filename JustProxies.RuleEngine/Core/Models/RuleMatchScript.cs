using System.Diagnostics;
using JustProxies.Proxy.Core.Internal;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace JustProxies.RuleEngine.Core.Models;

[DebuggerDisplay("执行代码: {Code}")]
public class RuleMatchScript(string code)
{
    public readonly string Code = code;

    /// <summary>
    /// 执行编译
    /// </summary>
    public Func<HttpContext, bool> Compile()
    {
        var script = CSharpScript.Create<bool>(Code, null, typeof(HttpContext));
        script.Compile();
        this.CompiledScript = (request) => script.RunAsync(request, OnError).Result.ReturnValue;
        return CompiledScript;
    }

    /// <summary>
    /// 编译后委托脚本
    /// </summary>
    public Func<HttpContext, bool> CompiledScript { get; private set; } = _ => true;

    /// <summary>
    /// 代码异常时，返回内容
    /// </summary>
    public Func<Exception, bool> OnError { get; set; } = _ => false;
}