using CoingeckoCollector.Abstractions;
using CoingeckoCollector.Settings;
using Microsoft.Extensions.Options;

namespace CoingeckoCollector.Worker;

internal class ValuesCollectorWorker : BackgroundService
{
    private readonly ILogger<ValuesCollectorWorker> logger;
    private readonly IOptions<CoingeckoCollectorSettings> settings;
    private readonly ICollectorFlow flow;

    public ValuesCollectorWorker(
        ILogger<ValuesCollectorWorker> logger, IOptions<CoingeckoCollectorSettings> settings, ICollectorFlow flow)
    {
        this.logger = logger;
        this.settings = settings;
        this.flow = flow;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await flow.CollectAsync(stoppingToken);

            logger.LogInformation("Worker finished at: {time}", DateTimeOffset.Now);

            await Task.Delay(settings.Value.WorkerInterval, stoppingToken);
        }
    }
}