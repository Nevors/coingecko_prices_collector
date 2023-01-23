namespace MessageBroker.Contracts.Abstractions;

public interface IMessage<T>
{
    public T Value { get; }
}

public interface IConsumeMessage<T> : IMessage<T>
{
    public DateTime Timestamp { get; }
}