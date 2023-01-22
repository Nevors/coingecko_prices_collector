using Configuration.Extensions;
using Confluent.Kafka;
using Kafka.Abstractions;
using Kafka.Settings;
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
            var config = new ProducerConfig {
                BootstrapServers = settings.BootstrapServers
            };

            return new ProducerBuilder<byte[], byte[]>(config).Build();
        });

        return new(services, settings);
    }
}

public class KafkaConfigurator
{
    private readonly IServiceCollection services;
    private readonly KafkaSettings settings;

    public KafkaConfigurator(IServiceCollection services, KafkaSettings settings)
    {
        this.services = services;
        this.settings = settings;
    }

    public KafkaConfigurator AddEvent<T>()
    {
        services.AddSingleton<IProducer<Null, T>>(sp =>
        {
            var baseProducer = sp.GetRequiredService<IProducer<byte[], byte[]>>();

            return new DependentProducerBuilder<Null, T>(baseProducer.Handle)
                .SetValueSerializer(new JsonEventSerializer<T>())
                .Build();
        });

        var eventMapping = settings.Events.FirstOrDefault(map => map.Name == typeof(T).Name);
        if (eventMapping is null)
        {
            throw new ArgumentException($"Для события {typeof(T)} не задан маппинг на топик");
        }

        services.AddTransient<IEventProducer<T>>(sp =>
        {
            return ActivatorUtilities.CreateInstance<EventProducer<T>>(
                sp, new EventProducer<T>.Topic(eventMapping.TopicName));
        });

        return this;
    }
}
