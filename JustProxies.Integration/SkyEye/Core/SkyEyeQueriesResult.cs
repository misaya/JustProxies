namespace JustProxies.Integration.SkyEye;

public record SkyEyeQueriesResult
{
    public int Count { get; init; }
    public List<SkyEyeLogContent> List { get; init; } = null!;
}