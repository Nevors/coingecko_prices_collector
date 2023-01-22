using Confluent.Kafka;
using Kafka.Abstractions;
using MessageBroker.Contracts.Abstractions;

namespace Kafka;

internal class EventProducer<T> : IEventProducer<T>
{
    private readonly IProducer<Null, T> producer;
    private readonly Topic topic;

    public EventProducer(IProducer<Null, T> producer, Topic topic)
    {
        this.producer = producer;
        this.topic = topic;
    }

    public async Task Produce(IMessage<T> message, CancellationToken cancellationToken)
    {
        var kafkaMessage = new Message<Null, T>
        {
            Value = message.Value
        };

        await producer.ProduceAsync(topic.Name, kafkaMessage, cancellationToken);
    }

    public record Topic(string Name);
}
