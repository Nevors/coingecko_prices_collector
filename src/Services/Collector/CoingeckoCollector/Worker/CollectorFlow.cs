using AutoMapper;
using Coingecko.Api.Contracts;
using Coingecko.Api.Contracts.Dto.Coins;
using CoingeckoCollector.Abstractions;
using CoingeckoCollector.Settings;
using Common.Extensions;
using Kafka.Abstractions;
using MessageBroker.Contracts;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using СoingeckoKafka.Contracts;

namespace CoingeckoCollector.Worker
{
    internal class CollectorFlow : ICollectorFlow
    {
        private readonly CoingeckoCollectorSettings settings;
        private readonly ILogger<CollectorFlow> logger;
        private readonly ICoingeckoCoinsApi coinsApi;
        private readonly IEventProducer<CurrencyCoinsUpdatedEvent> eventProducer;
        private readonly IMapper mapper;

        public CollectorFlow(
            IOptions<CoingeckoCollectorSettings> settings,
            ILogger<CollectorFlow> logger,
            ICoingeckoCoinsApi coinsApi,
            IEventProducer<CurrencyCoinsUpdatedEvent> eventProducer,
            IMapper mapper)
        {
            this.settings = settings.Value;
            this.logger = logger;
            this.coinsApi = coinsApi;
            this.eventProducer = eventProducer;
            this.mapper = mapper;
        }

        public async Task CollectAsync(CancellationToken token)
        {
            await foreach (var part in Collect(token))
            {
                var countOfCoins = part.Coins.Count();

                logger.LogInformation("Для {Currency} загружено {Count} монет", part.Currency, countOfCoins);

                if (countOfCoins != 0)
                {
                    var message = new EventMessage<CurrencyCoinsUpdatedEvent>(mapper.Map<CurrencyCoinsUpdatedEvent>(part));

                    await eventProducer.Produce(message, token);
                }
            }
        }

        record Part(string Currency, IEnumerable<CoinDto> Coins);

        private async IAsyncEnumerable<Part> Collect([EnumeratorCancellation] CancellationToken token)
        {
            var coinsPartsTasks = settings.Currencies
                .Select(async currency => (Currency: currency, CoinsTasks: await coinsApi.GetAllPricesAsync(currency, token)));

            var coinsParts = await Task.WhenAll(coinsPartsTasks);
            var currencyCoins = coinsParts
                .SelectMany(_ => _.CoinsTasks, async (part, tasks) => new Part(part.Currency, await tasks))
                .ToArray();

            foreach (var coins in currencyCoins.OrderByCompletion())
            {
                yield return await coins;
            }
        }

        public class KafkaMessagesProfile : Profile
        {
            public KafkaMessagesProfile()
            {
                CreateMap<Part, CurrencyCoinsUpdatedEvent>()
                    .ForMember(e => e.CurrencyCoins, ctx => ctx.MapFrom(m => new[] { m }));

                CreateMap<Part, CurrencyCoins>()
                    .ConstructUsing(m => new(m.Currency));

                CreateMap<CoinDto, Coin>()
                    .ConstructUsing(coinDto => new(coinDto.Symbol, coinDto.CurrentPrice));
            }
        }
    }
}
