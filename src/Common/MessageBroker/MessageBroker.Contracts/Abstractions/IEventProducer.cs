namespace MessageBroker.Contracts.Abstractions
{
    public interface IEventProducer<T>
    {
        Task Produce(IMessage<T> message, CancellationToken cancellationToken);
    }
}
