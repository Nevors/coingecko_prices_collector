using Coingecko.Api.Abstractions.Queries;
using Coingecko.Api.Contracts.Dto.Coins;
using Microsoft.Extensions.Logging;

namespace Coingecko.Api.Queries;

internal class CurrencyCoinsPageQuerySuppressDecorator : ICurrencyCoinsPageQuery
{
    private readonly ICurrencyCoinsPageQuery inner;
    private readonly ILogger<CurrencyCoinsPageQuerySuppressDecorator> logger;

    public CurrencyCoinsPageQuerySuppressDecorator(
        ICurrencyCoinsPageQuery inner,
        ILogger<CurrencyCoinsPageQuerySuppressDecorator> logger)
    {
        this.inner = inner;
        this.logger = logger;
    }

    public async Task<IEnumerable<CoinDto>> GetCoinsPageAsync(ICurrencyCoinsPageQuery.Request request, CancellationToken token)
    {
        try
        {
            return await inner.GetCoinsPageAsync(request, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Ошибка при поулчении страницы. {Request}", request);
            return Enumerable.Empty<CoinDto>();
        }
    }
}
