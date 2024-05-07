namespace JustProxies.Integration.SkyEye;

public record SkyEyeQueriesResult
{
    public int Count { get; init; }
    public List<SkyEyeContent> List { get; init; } = null!;
}