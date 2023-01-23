using Kafka.Settings;
using MessageBroker.Contracts.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka.Subscriber;

internal class EventSubscriberWorker<T> : BackgroundService
{
    private readonly IEventConsumer<T> consumer;
    private readonly IEventHandler<T> eventHandler;
    private readonly KafkaSettings kafkaSettings;
    private readonly ILogger<EventSubscriberWorker<T>> logger;

    public EventSubscriberWorker(
        IEventConsumer<T> consumer,
        IEventHandler<T> eventHandler,
        KafkaSettings kafkaSettings,
        ILogger<EventSubscriberWorker<T>> logger)
    {
        this.consumer = consumer;
        this.eventHandler = eventHandler;
        this.kafkaSettings = kafkaSettings;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunIteration(stoppingToken);
        }
    }

    private async Task RunIteration(CancellationToken token)
    {
        try
        {
            var consumeResult = await consumer.Consume(token);

            await eventHandler.Handle(consumeResult.Message, consumeResult.Commit, token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при считывание сообщений из топика. Приостановка на {ErrorTimeout}", kafkaSettings.ErrorTimeout);

            await Task.Delay(kafkaSettings.ErrorTimeout);
        }
    }
}
