using JustProxies.Integration.SkyEye;

namespace JustProxies.Integration.Core;

public interface ISkyEyeIntegration
{
    Task<List<string>> QueryAppUkListAsync();

    Task<List<SkyEyeContent>> QueryAsync(string appUk,
        SkyEyeQueries queries);
}