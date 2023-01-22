using MessageBroker.Contracts.Abstractions;

namespace Kafka.Abstractions
{
    public interface IEventProducer<T>
    {
        Task Produce(IMessage<T> message, CancellationToken cancellationToken);
    }
}
