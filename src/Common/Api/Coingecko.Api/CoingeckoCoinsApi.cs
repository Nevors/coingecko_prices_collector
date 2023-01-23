using Coingecko.Api.Abstractions.Queries;
using Coingecko.Api.Contracts;
using Coingecko.Api.Contracts.Dto.Coins;

namespace Coingecko.Api;

internal class CoingeckoCoinsApi : ICoingeckoCoinsApi
{
    private readonly ICurrencyCoinsPageQuery currencyCoins;
    private readonly ISuppotedCoinsQuery suppotedCoins;

    private const int pageSize = 250;

    public CoingeckoCoinsApi(
        ICurrencyCoinsPageQuery currencyCoins, ISuppotedCoinsQuery suppotedCoins)
    {
        this.currencyCoins = currencyCoins;
        this.suppotedCoins = suppotedCoins;
    }

    public async Task<IEnumerable<Task<IEnumerable<CoinDto>>>> GetAllPricesAsync(string currency, CancellationToken token)
    {
        var countOfCoins = await suppotedCoins.GetCountAsync(token);
        var countOfPages = (countOfCoins + pageSize - 1) / pageSize;

        return Enumerable
            .Range(1, countOfPages)
            .Select(pageNumber => currencyCoins.GetCoinsPageAsync(
                        new(currency, PageNumber: pageNumber, PageSize: pageSize), token))
            .ToArray();
    }
}
