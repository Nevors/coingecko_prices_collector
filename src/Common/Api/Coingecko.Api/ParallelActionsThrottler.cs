using Coingecko.Api.Abstractions;
using Coingecko.Api.Settings;

namespace Coingecko.Api;

internal class ParallelActionsThrottler<T> : IParallelActionsThrottler<T>
{
    private readonly SemaphoreSlim semaphore;

    public ParallelActionsThrottler(CoingeckoApiSettings settings)
    {
        var initial = settings.MaxParallelActions.TryGetValue(typeof(T).Name, out var maxParrallelActions)
            ? maxParrallelActions
            : settings.DefaultMaxParallelActions;

        semaphore = new SemaphoreSlim(initial);
    }

    public async Task<TResult> Do<TResult>(Func<Task<TResult>> func, CancellationToken token)
    {
        try
        {
            await semaphore.WaitAsync(token);

            return await func();
        }
        finally
        {
            semaphore.Release();
        }
    }
}
