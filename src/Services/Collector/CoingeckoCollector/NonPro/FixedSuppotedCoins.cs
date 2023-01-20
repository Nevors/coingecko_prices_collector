using Coingecko.Api.Abstractions.Queries;
using CoingeckoCollector.Settings;
using Microsoft.Extensions.Options;

namespace CoingeckoCollector.NonPro;

internal class FixedSuppotedCoins : ISuppotedCoinsQuery
{
    private readonly CoingeckoCollectorSettings settings;

    public FixedSuppotedCoins(IOptions<CoingeckoCollectorSettings> options)
    {
        settings = options.Value;
    }

    public Task<int> GetCountAsync(CancellationToken token)
    {
        var maxCountOfRequests = 100;
        var avgCountOfPages = maxCountOfRequests / settings.Currencies.Length;
        var avgCountOfCoinsForCurrenceis = avgCountOfPages * 250;

        return Task.FromResult(avgCountOfCoinsForCurrenceis);
    }
}
