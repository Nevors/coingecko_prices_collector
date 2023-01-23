using Coingecko.Events.Contracts;
using MessageBroker.Contracts.Abstractions;

namespace CoingeckoPrices.WebApi.EventHandler
{
    public class CoinsUpdatedEventHandlerLoggerDecorator : IEventHandler<CurrencyCoinsUpdatedEvent>
    {
        private readonly IEventHandler<CurrencyCoinsUpdatedEvent> inner;
        private readonly ILogger<CoinsUpdatedEventHandler> logger;

        public CoinsUpdatedEventHandlerLoggerDecorator(
            IEventHandler<CurrencyCoinsUpdatedEvent> inner,
            ILogger<CoinsUpdatedEventHandler> logger)
        {
            this.inner = inner;
            this.logger = logger;
        }

        public Task Handle(IConsumeMessage<CurrencyCoinsUpdatedEvent> message, Action commit, CancellationToken cancellationToken)
        {
            var countOfCoins = message.Value.CurrencyCoins.Aggregate(0, (sum, cur) => sum + cur.Coins.Count);

            logger.LogInformation("Получены обновления для {Count} монет", countOfCoins);

            return inner.Handle(message, commit, cancellationToken);
        }
    }
}
