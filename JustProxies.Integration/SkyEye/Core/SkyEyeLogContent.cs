namespace JustProxies.Integration.SkyEye;

public class SkyEyeLogContent
{
    public string LogTime { get; set; } = null!;
    public long Timestamp { get; set; }

    public string Module { get; set; } = null!;

    public string Category { get; set; } = null!;
    public string SubCategory { get; set; } = null!;

    /// <summary>
    /// 日志级别(0:FATAL,1:ERROR,2:WARN,3:INFO,4:DEBUG)
    /// </summary>
    public SkyEyeLogLevel Priority { get; set; }

    public string Filter1 { get; set; } = null!;
    public string Filter2 { get; set; } = null!;
    public string ContextId { get; set; } = null!;
    public string Msg { get; set; } = null!;

    public string Ip { get; set; } = null!;

    public string Env { get; set; } = null!;

    public bool Kms { get; set; } = false;
    public string LogicIdcUk { get; set; } = null!;
    public int AppId { get; set; }
    public string DomainName { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string AppUk { get; set; } = null!;

    public string ExtraInfo { get; set; } = null!;
}