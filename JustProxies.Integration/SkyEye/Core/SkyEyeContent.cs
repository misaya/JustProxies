namespace JustProxies.Integration.SkyEye;

public record SkyEyeContent
{
    public string Msg { get; init; } = null!;
    public string SubCategory { get; init; } = null!;
    public string Module { get; init; } = null!;
    public string Ip { get; init; } = null!;
    public string Filter1 { get; init; } = null!;
    public string Filter2 { get; init; } = null!;
    public string ContextId { get; init; } = null!;
    public int Priority { get; init; }
    public string Env { get; init; } = null!;
    public string LogTime { get; init; } = null!;
    public bool Kms { get; init; } = false;
    public string LogicIdcUk { get; init; } = null!;
    public int AppId { get; init; }
    public string DomainName { get; init; } = null!;
    public string Id { get; init; } = null!;
    public string AppUk { get; init; } = null!;
    public string Category { get; init; } = null!;
    public string ExtraInfo { get; init; } = null!;
    public long Timestamp { get; init; }
}