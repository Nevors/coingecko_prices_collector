using Kafka.Extensions;
using MessageBroker.Contracts.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.Subscriber.Extensions;

public static class ServiceCollectionExtensions
{
    public static KafkaConfigurator AddEventSubscriber<T, TSubscriber>(this KafkaConfigurator kafkaConfurator)
        where TSubscriber : class, IEventHandler<T>
    {
        kafkaConfurator.Services.AddTransient<IEventHandler<T>, TSubscriber>();
        kafkaConfurator.Services.AddHostedService<EventSubscriberWorker<T>>();

        return kafkaConfurator;
    }
}