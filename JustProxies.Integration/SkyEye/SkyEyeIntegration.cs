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

    public Task<List<string>> QueryAppUkListAsync()
    {
        var result = this._options.AppList.Select(p => p.AppUk).ToList();
        return Task.FromResult(result);
    }

    public async Task<List<SkyEyeContent>> QueryAsync(string appUk, SkyEyeQueries query)
    {
        try
        {
            var app = this._options.AppList.Find(p => p.AppUk == appUk)!;
            if (app.IsNull())
            {
                throw new ArgumentOutOfRangeException(nameof(appUk), $"{appUk}未配置appId及token");
            }

            query.AddApp(app.AppId, app.Token);

            var httpclient = httpClientFactory.CreateClient();
            httpclient.BaseAddress = new Uri(this._options.BaseUrl);
            var result = await
                httpclient.PostAsync(Constants.QueryRealTimeLogUrl, query.GetHttpContent(), CancellationToken.None);
            result.EnsureSuccessStatusCode();
            var text = await result.Content.ReadAsStringAsync();

            var resp = text.ToObject<SkyEyeRespRoot>(true);
            if (resp!.Code == 200 && resp.Result is { Count: > 0, List.Count: > 0 })
            {
                return resp.Result.List!;
            }
            else
            {
                throw new Exception($"返回内容:{text}");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "查询失败");
            return null!;
        }
    }
}