using CoingeckoPrices.WebApi.Storage.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace CoingeckoPrices.WebApi.Storage;

public class CoinsStorage : ICoinsStorage
{
    private readonly Dictionary<string, Dictionary<string, Coin>> currencyCoins = new(StringComparer.InvariantCultureIgnoreCase);

    public Task AddOrUpdate(string currency, IEnumerable<Coin> coins, CancellationToken token)
    {
        var currentCoins = currencyCoins.TryGetValue(currency, out var linkedCoins)
            ? linkedCoins
            : currencyCoins[currency] = new Dictionary<string, Coin>(StringComparer.InvariantCultureIgnoreCase);

        foreach (var coin in coins)
        {
            currentCoins[coin.Symbol] = coin;
        }

        return Task.CompletedTask;
    }

    public Task<Coin?> GetCoin(string currency, string coinName, CancellationToken token)
    {
        var currentCoins = currencyCoins.TryGetValue(currency, out var linkedCoins)
            ? linkedCoins
            : throw new CurrencyNotFoundException(currency);

        return Task.FromResult(currentCoins.TryGetValue(coinName, out var coin) ? coin : default);
    }

    public IAsyncEnumerable<Coin> GetCoins(string currency, CancellationToken token)
    {
        return currencyCoins.TryGetValue(currency, out var coins)
            ? coins.Values.ToAsyncEnumerable()
            : throw new CurrencyNotFoundException(currency);
    }

    public Task<IEnumerable<string>> GetCurrencies(CancellationToken token)
        => Task.FromResult(currencyCoins.Keys.AsEnumerable());

    class CoinComparer : IEqualityComparer<Coin>
    {
        public bool Equals(Coin? x, Coin? y) => x?.Symbol == y?.Symbol;

        public int GetHashCode([DisallowNull] Coin obj) => obj.Symbol.GetHashCode();
    }
}
