namespace JustProxies.Integration.SkyEye;

public record SkyEyeRespRoot
{
    public int Code { get; init; }
    public string Message { get; init; } = null!;
    public SkyEyeQueriesResult Result { get; init; } = null!;
}