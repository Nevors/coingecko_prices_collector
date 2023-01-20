namespace Coingecko.Api.Contracts.Dto.Coins;

public record CoinDto
{
    public string Symbol { get; init; } = string.Empty;

    public double? CurrentPrice { get; init; }
}
