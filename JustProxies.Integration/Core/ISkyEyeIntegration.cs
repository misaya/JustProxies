using JustProxies.Integration.SkyEye;

namespace JustProxies.Integration.Core;

public interface ISkyEyeIntegration
{
    Task<SkyEyeRespRoot> QueryAsync(SkyEyeQueries queries);
}