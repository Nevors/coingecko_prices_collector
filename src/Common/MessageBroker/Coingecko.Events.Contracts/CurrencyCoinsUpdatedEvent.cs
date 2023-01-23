namespace Coingecko.Events.Contracts;

public record CurrencyCoinsUpdatedEvent
{
    public IReadOnlyCollection<CurrencyCoins> CurrencyCoins { get; init; } = Array.Empty<CurrencyCoins>();
}

public record CurrencyCoins(string Currency)
{
    public IReadOnlyCollection<Coin> Coins { get; init; } = Array.Empty<Coin>();
}

public record Coin(string Symbol, double Price);
