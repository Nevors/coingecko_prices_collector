namespace Coingecko.Api.Abstractions;

public interface IGlobalMemmoryCache<TMarker>
{
    Task<T> Get<T>(Func<Task<T>> func, CancellationToken token);
}
