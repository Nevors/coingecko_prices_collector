namespace CoingeckoPrices.WebApi.Storage.Contracts;

public interface ICoinsStorage
{
    Task AddOrUpdate(string currency, IEnumerable<Coin> coins, CancellationToken token);

    IAsyncEnumerable<Coin> GetCoins(string currency, CancellationToken token);

    Task<IEnumerable<string>> GetCurrencies(CancellationToken token);

    Task<Coin?> GetCoin(string currency, string coin, CancellationToken token);
}

public record Coin(string Symbol, double Price, DateTime UpdatedAt);

public class CurrencyNotFoundException : Exception
{
    public CurrencyNotFoundException(string currency)
        : base("Базовая валюта не найдена")
    {
        Currency = currency;
    }

    public string Currency { get; }
}

public class CurrencyCoinNotFoundException : Exception
{
    public CurrencyCoinNotFoundException(string currency, string coin)
        : base($"Монета {coin} по отношению к базовой валюты {currency} не найдена")
    {
        Currency = currency;
        Coin = coin;
    }

    public string Currency { get; }

    public string Coin { get; }
}