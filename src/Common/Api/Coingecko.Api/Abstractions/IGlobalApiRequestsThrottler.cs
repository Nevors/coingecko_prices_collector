namespace Coingecko.Api.Abstractions;

public interface IGlobalApiRequestsThrottler
{
    Task<T> Do<T>(Func<Task<T>> func, CancellationToken token);
}
