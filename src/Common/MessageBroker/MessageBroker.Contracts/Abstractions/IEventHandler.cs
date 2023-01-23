namespace MessageBroker.Contracts.Abstractions;

public interface IEventHandler<T>
{
    Task Handle(IConsumeMessage<T> message, Action commit, CancellationToken cancellationToken);
}
