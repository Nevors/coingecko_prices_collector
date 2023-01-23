using CoingeckoPrices.WebApi.Storage.Contracts;
using MessageBroker.Contracts.Abstractions;
using Events = Coingecko.Events.Contracts;

namespace CoingeckoPrices.WebApi.EventHandler;

public class CoinsUpdatedEventHandler : IEventHandler<Events.CurrencyCoinsUpdatedEvent>
{
    private readonly ICoinsStorage storage;

    public CoinsUpdatedEventHandler(ICoinsStorage storage)
    {
        this.storage = storage;
    }

    public async Task Handle(IConsumeMessage<Events.CurrencyCoinsUpdatedEvent> message, Action commit, CancellationToken cancellationToken)
    {
        foreach (var currencyCoins in message.Value.CurrencyCoins)
        {
            var coins = currencyCoins.Coins.Select(coin => new Coin(coin.Symbol, coin.Price, message.Timestamp));

            await storage.AddOrUpdate(currencyCoins.Currency, coins, cancellationToken);
        }

        commit();
    }
}
