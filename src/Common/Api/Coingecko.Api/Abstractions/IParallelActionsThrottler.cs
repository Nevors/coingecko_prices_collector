namespace Coingecko.Api.Abstractions;

public interface IParallelActionsThrottler<T>
{
    Task<TResult> Do<TResult>(Func<Task<TResult>> func, CancellationToken token);
}
