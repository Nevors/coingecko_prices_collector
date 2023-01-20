namespace Coingecko.Api.Settings;

public record CoingeckoApiSettings
{
    public string BaseUrl { get; init; } = "https://api.coingecko.com/api/v3";

    public string ProBaseUrl { get; init; } = "https://pro-api.coingecko.com/api/v3/";

    public string? ApiKey { get; init; }

    public int DefaultMaxParallelActions { get; init; } = 10;

    public IDictionary<string, int> MaxParallelActions { get; init; } = new Dictionary<string, int>();

    public TimeSpan DefaultMemmoryCacheExpiration { get; init; } = TimeSpan.FromMinutes(5);

    public IDictionary<string, TimeSpan> MemmoryCacheExpiration { get; init; } = new Dictionary<string, TimeSpan>();

    public bool SuppressLoadingCoinsPageErrors { get; init; } = false;

    public bool WaitInCaseOfTooManyRequests { get; init; } = false;
}
