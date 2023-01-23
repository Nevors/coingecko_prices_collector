using Configuration.Extensions;
using Confluent.Kafka;
using Kafka.Settings;
using MessageBroker.Contracts.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static KafkaConfigurator AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.ConfigureSettings<KafkaSettings, KafkaSettingsValidator>(configuration);

        services.AddSingleton<IProducer<byte[], byte[]>>(sp =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = settings.BootstrapServers
            };

            return new ProducerBuilder<byte[], byte[]>(config).Build();
        });


        return new(services, settings);
    }
}

public class KafkaConfigurator
{
    public IServiceCollection Services { get; }
    public KafkaSettings Settings { get; }

    public KafkaConfigurator(IServiceCollection services, KafkaSettings settings)
    {
        Services = services;
        Settings = settings;
    }

    public KafkaConfigurator AddEvent<T>()
    {
        Services.AddSingleton<IProducer<Null, T>>(sp =>
        {
            var baseProducer = sp.GetRequiredService<IProducer<byte[], byte[]>>();

            return new DependentProducerBuilder<Null, T>(baseProducer.Handle)
                .SetValueSerializer(new JsonEventSerializer<T>())
                .Build();
        });

        var eventSettings = Settings.Events.FirstOrDefault(map => map.Name == typeof(T).Name);
        if (eventSettings is null)
        {
            throw new ArgumentException($"Для события {typeof(T)} не задан маппинг на топик");
        }

        Services.AddTransient<IConsumer<Null, T>>(sp =>
        {
            if (string.IsNullOrWhiteSpace(eventSettings.GroupId))
            {
                throw new ArgumentException($"Для события {typeof(T)} не задан {nameof(eventSettings.GroupId)}");
            }

            var config = new ConsumerConfig
            {
                BootstrapServers = Settings.BootstrapServers,
                GroupId = eventSettings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = false,
            };

            var consumer = new ConsumerBuilder<Null, T>(config)
                .SetValueDeserializer(new JsonEventDeserializer<T>())
                .Build();

            consumer.Subscribe(eventSettings.TopicName);

            return consumer;
        });

        Services.AddTransient<IEventProducer<T>>(sp =>
        {
            return ActivatorUtilities.CreateInstance<EventProducer<T>>(
                sp, new Topic(eventSettings.TopicName));
        });

        Services.AddTransient<IEventConsumer<T>, EventConsumer<T>>();

        return this;
    }
}
