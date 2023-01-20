using Coingecko.Api.Contracts.Dto.Coins;

namespace Coingecko.Api.Abstractions.Queries;

public interface ICurrencyCoinsPageQuery
{
    Task<IEnumerable<CoinDto>> GetCoinsPageAsync(Request request, CancellationToken token);

    public record Request(string Currency, int PageSize = 250, int PageNumber = 1);
}
