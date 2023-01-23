using Coingecko.Events.Contracts;
using CoingeckoPrices.WebApi.EventHandler;
using CoingeckoPrices.WebApi.Storage;
using CoingeckoPrices.WebApi.Storage.Contracts;
using Kafka.Extensions;
using Kafka.Subscriber.Extensions;
using MessageBroker.Contracts.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddKafka(builder.Configuration)
    .AddEvent<CurrencyCoinsUpdatedEvent>()
    .AddEventSubscriber<CurrencyCoinsUpdatedEvent, CoinsUpdatedEventHandler>();

builder.Services
    .AddTransient<IConversionQuery, ConversionQuery>()
    .AddSingleton<ICoinsStorage, CoinsStorage>()
    .Decorate<IEventHandler<CurrencyCoinsUpdatedEvent>, CoinsUpdatedEventHandlerLoggerDecorator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/currencies/{currency}", (ICoinsStorage storage, string currency, CancellationToken token) =>
{
    return storage.GetCoins(currency, token);
})
.WithName("GetCurrenciesCoins");

app.MapGet("/coins/{base}_{quote}", (IConversionQuery query, string @base, string quote, CancellationToken token) =>
{
    return query.ComputeConversion(@base, quote, token);
})
.WithName("GetConversionOfPair");

app.Run();