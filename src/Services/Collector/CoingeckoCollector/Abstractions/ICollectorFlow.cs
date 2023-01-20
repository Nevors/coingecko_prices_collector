namespace CoingeckoCollector.Abstractions;

internal interface ICollectorFlow
{
    Task CollectAsync(CancellationToken token);
}
