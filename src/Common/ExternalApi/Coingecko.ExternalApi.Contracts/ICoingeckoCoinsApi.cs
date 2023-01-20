using Coingecko.Api.Contracts.Dto.Coins;

namespace Coingecko.Api.Contracts;

public interface ICoingeckoCoinsApi
{
    /// <summary>
    /// Получить все цены для <paramref name="currency"/>.
    /// Потенциально данных может быть много поэтому имеет смысл дать возможность получать данные порционно.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IEnumerable<Task<IEnumerable<CoinDto>>>> GetAllPricesAsync(string currency, CancellationToken token);
}
