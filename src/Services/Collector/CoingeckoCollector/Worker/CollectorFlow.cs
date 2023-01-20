using Coingecko.Api.Contracts;
using Coingecko.Api.Contracts.Dto.Coins;
using CoingeckoCollector.Abstractions;
using CoingeckoCollector.Settings;
using Common.Extensions;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace CoingeckoCollector.Worker
{
    internal class CollectorFlow : ICollectorFlow
    {
        private readonly CoingeckoCollectorSettings settings;
        private readonly ICoingeckoCoinsApi coinsApi;

        public CollectorFlow(IOptions<CoingeckoCollectorSettings> settings, ICoingeckoCoinsApi coinsApi)
        {
            this.settings = settings.Value;
            this.coinsApi = coinsApi;
        }

        public async Task CollectAsync(CancellationToken token)
        {
            await foreach (var part in Collect(token))
            {
                Console.WriteLine($"{part.Currency}: {part.Coins.Count()}");
            }
        }

        record Message(string Currency, IEnumerable<CoinDto> Coins);

        private async IAsyncEnumerable<Message> Collect([EnumeratorCancellation] CancellationToken token)
        {
            var coinsPartsTasks = settings.Currencies
                .Select(async currency => (Currency: currency, CoinsTasks: await coinsApi.GetAllPricesAsync(currency, token)));

            var coinsParts = await Task.WhenAll(coinsPartsTasks);
            var currencyCoins = coinsParts
                .SelectMany(_ => _.CoinsTasks, async (part, tasks) => new Message(part.Currency, await tasks))
                .ToArray();

            foreach (var coins in currencyCoins.OrderByCompletion())
            {
                yield return await coins;
            }
        }
    }
}
