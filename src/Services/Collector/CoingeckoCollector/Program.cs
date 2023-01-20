using Coingecko.Api.Abstractions.Queries;
using Coingecko.Api.Extensions;
using Coingecko.Api.Settings;
using CoingeckoCollector.Abstractions;
using CoingeckoCollector.NonPro;
using CoingeckoCollector.Settings;
using CoingeckoCollector.Worker;
using RestSharp;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddCoingeckoApi(
            settings => context.Configuration.GetSection(nameof(CoingeckoApiSettings)).Bind(settings));

        var section = context.Configuration.GetSection(nameof(CoingeckoCollectorSettings));
        var settings = section.Get<CoingeckoCollectorSettings>();
        services.Configure<CoingeckoCollectorSettings>(section);

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

record Success(string Protocol, string Host, string Content);