using Coingecko.Api.Abstractions;
using Coingecko.Api.Settings;

namespace Coingecko.Api;

internal class GlobalCache<TMarker> : IGlobalMemmoryCache<TMarker>
{
    private readonly TimeSpan cacheExpiration;

    public GlobalCache(CoingeckoApiSettings settings)
    {
        cacheExpiration = settings.MemmoryCacheExpiration.TryGetValue(nameof(TMarker), out var expiration)
            ? expiration
            : settings.DefaultMemmoryCacheExpiration;
    }

    private DateTime? lastUpdate;

    // TODO: подумать над боксингом структур
    private object? lastCached;

    private TaskCompletionSource? taskCompletion;

    public async Task<T> Get<T>(Func<Task<T>> func, CancellationToken token)
    {
        if (lastUpdate.HasValue && lastCached is not null && lastUpdate.Value.Add(cacheExpiration) > DateTime.UtcNow)
        {
            return (T)lastCached;
        }

        var ownTaskCompletion = Interlocked.CompareExchange(
                ref taskCompletion, new TaskCompletionSource(), null);

        if (ownTaskCompletion != null)
        {
            await ownTaskCompletion.Task.WaitAsync(token);
            return (T)lastCached!;
        }

        try
        {
            lastCached = await func();
            lastUpdate = DateTime.UtcNow;

            taskCompletion.SetResult();

            return (T)lastCached!;
        }
        catch (Exception ex)
        {
            taskCompletion?.SetException(ex);

            throw;
        }
        finally
        {
            taskCompletion = default;
        }
    }
}
