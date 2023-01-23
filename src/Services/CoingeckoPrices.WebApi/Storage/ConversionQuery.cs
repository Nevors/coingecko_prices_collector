using CoingeckoPrices.WebApi.Storage.Contracts;

namespace CoingeckoPrices.WebApi.Storage;

public class ConversionQuery : IConversionQuery
{
    private readonly ICoinsStorage coinsStorage;

    public ConversionQuery(ICoinsStorage coinsStorage)
    {
        this.coinsStorage = coinsStorage;
    }

    public async Task<double> ComputeConversion(string baseCoin, string quoteCoin, CancellationToken token)
    {
        var currencies = await coinsStorage.GetCurrencies(token);

        var pair = await currencies
            .ToAsyncEnumerable()
            .SelectAwait(async currency => new
            {
                Base = await coinsStorage.GetCoin(currency, baseCoin, token),
                Quote = await coinsStorage.GetCoin(currency, quoteCoin, token)
            })
            .Where(pair => pair.Quote != null && pair.Base != null)
            .FirstOrDefaultAsync(token);

        if (pair == null)
        {
            throw new PairNotFoundException(baseCoin, quoteCoin);
        }

        return pair.Base!.Price / pair.Quote!.Price;
    }
}
