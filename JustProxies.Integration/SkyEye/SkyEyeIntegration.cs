using JustProxies.Common;
using JustProxies.Integration.Core;
using JustProxies.Integration.SkyEye.Exts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JustProxies.Integration.SkyEye;

public class SkyEyeIntegration(
    IOptions<SkyEyeOptions> options,
    ILogger<SkyEyeIntegration> logger,
    IHttpClientFactory httpClientFactory) : ISkyEyeIntegration
{
    private readonly SkyEyeOptions _options = options.Value;

    public async Task<SkyEyeRespRoot> QueryAsync(SkyEyeQueries query)
    {
        try
        {
            if (query.AppIds.IsEmpty())
            {
                foreach (var app in this._options.AppList)
                {
                    query.AddApp(app.AppId, app.Token);
                }
            }

            var httpclient = httpClientFactory.CreateClient();
            httpclient.BaseAddress = new Uri(this._options.BaseUrl);
            var result = await
                httpclient.PostAsync(Constants.QueryRealTimeLogUrl, query.GetHttpContent(), CancellationToken.None);
            result.EnsureSuccessStatusCode();
            var text = await result.Content.ReadAsStringAsync();

            var resp = text.ToObject<SkyEyeRespRoot>(true);
            return resp!;
        }
        catch (Exception e)
        {
            logger.LogError(e, "查询失败");
            return null!;
        }
    }
}