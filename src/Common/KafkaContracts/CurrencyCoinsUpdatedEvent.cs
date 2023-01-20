namespace KafkaContracts;

public record CurrencyCoinsUpdatedEvent
{
    public IEnumerable<CurrencyCoins> CurrencyCoins { get; init; } = Enumerable.Empty<CurrencyCoins>();
}

public record CurrencyCoins(string Currency)
{
    public IEnumerable<Coin> Coins { get; init; } = Enumerable.Empty<Coin>();
}

public record Coin(string Symbol, double Price);
