namespace MessageBroker.Contracts.Abstractions;

public interface IMessage<T>
{
    public T Value { get; }
}
