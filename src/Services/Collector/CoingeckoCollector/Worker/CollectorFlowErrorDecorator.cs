using CoingeckoCollector.Abstractions;

namespace CoingeckoCollector.Worker;

internal class CollectorFlowErrorDecorator : ICollectorFlow
{
    private readonly ICollectorFlow inner;
    private readonly ILogger<CollectorFlowErrorDecorator> logger;

    public CollectorFlowErrorDecorator(ICollectorFlow inner, ILogger<CollectorFlowErrorDecorator> logger)
    {
        this.inner = inner;
        this.logger = logger;
    }

    public async Task CollectAsync(CancellationToken token)
    {
        try
        {
            await inner.CollectAsync(token);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Ошибка при сборе данныих из АПИ");
        }
    }
}
