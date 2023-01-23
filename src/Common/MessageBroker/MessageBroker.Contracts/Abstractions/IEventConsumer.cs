namespace MessageBroker.Contracts.Abstractions;

public interface IEventConsumer<T> : IDisposable
{
    Task<IConsumeResult> Consume(CancellationToken token);

    void Commit(IConsumeResult consumeResult);

    public interface IConsumeResult
    {
        public IConsumeMessage<T> Message { get; }

        public void Commit();
    }
}

public class EventDeserializationException : Exception
{
    public string RawMessage { get; }

    public EventDeserializationException(string rawMessage, Exception? ex = null)
        : base("Ошибка десериализации сообщения", ex)
    {
        RawMessage = rawMessage;
    }
}