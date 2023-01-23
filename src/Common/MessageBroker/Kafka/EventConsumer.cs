using Confluent.Kafka;
using MessageBroker.Contracts;
using MessageBroker.Contracts.Abstractions;

namespace Kafka;

internal class EventConsumer<T> : IEventConsumer<T>
{
    private readonly IConsumer<Null, T> consumer;

    public EventConsumer(IConsumer<Null, T> consumer)
    {
        this.consumer = consumer;
    }

    public void Commit(IEventConsumer<T>.IConsumeResult consumeResult) => consumeResult.Commit();

    public async Task<IEventConsumer<T>.IConsumeResult> Consume(CancellationToken token)
    {
        var consumeResult = await Task.Run(() => consumer.Consume(token), token);

        var message = new EventConsumeMessage<T>(
            Value: consumeResult.Message.Value,
            Timestamp: consumeResult.Message.Timestamp.UtcDateTime);

        return new ConsumerResult(message, consumeResult, consumer);
    }

    public void Dispose()
    {
        consumer.Dispose();
    }

    class ConsumerResult : IEventConsumer<T>.IConsumeResult
    {
        private readonly ConsumeResult<Null, T> consumeResult;
        private readonly IConsumer<Null, T> consumer;

        public IConsumeMessage<T> Message { get; }

        public ConsumerResult(
            IConsumeMessage<T> message,
            ConsumeResult<Null, T> consumeResult,
            IConsumer<Null, T> consumer)
        {
            Message = message;

            this.consumeResult = consumeResult;
            this.consumer = consumer;
        }

        public void Commit()
        {
            consumer.Commit(consumeResult);
        }
    }
}
