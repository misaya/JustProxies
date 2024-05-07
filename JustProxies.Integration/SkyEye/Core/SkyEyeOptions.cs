namespace JustProxies.Integration.SkyEye;

public class SkyEyeOptions
{
    public const string OptionsName = nameof(SkyEyeOptions);
    public string BaseUrl { get; init; } = string.Empty;
    public List<SkyNetAppOptions> AppList { get; init; } = [];

    public class SkyNetAppOptions
    {
        public string AppId { get; init; } = string.Empty;
        public string AppUk { get; init; } = string.Empty;

        public string AppName { get; init; } = string.Empty;
        public string Token { get; init; } = string.Empty;
    }
}