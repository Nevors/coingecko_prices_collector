using Coingecko.Api.Abstractions.Queries;
using Coingecko.Api.Extensions;
using CoingeckoCollector.Abstractions;
using CoingeckoCollector.NonPro;
using CoingeckoCollector.Settings;
using CoingeckoCollector.Worker;
using Configuration.Extensions;
using Kafka.Extensions;
using System.Reflection;
using ÑoingeckoKafka.Contracts;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var settings = services.ConfigureSettings<CoingeckoCollectorSettings, CoingeckoCollectorSettingsValidator>(context.Configuration);

        services.AddKafka(context.Configuration)
            .AddEvent<CurrencyCoinsUpdatedEvent>();

        services.AddCoingeckoApi(context.Configuration);

        services.AddAutoMapper(Assembly.GetEntryAssembly());

        services.AddTransient<ICollectorFlow, CollectorFlow>();
        services.Decorate<ICollectorFlow, CollectorFlowErrorDecorator>();

        services.AddHostedService<ValuesCollectorWorker>();

        if (settings.UsePro == false)
        {
            services.AddTransient<ISuppotedCoinsQuery, FixedSuppotedCoins>();
        }
    })
    .Build();

await host.RunAsync();