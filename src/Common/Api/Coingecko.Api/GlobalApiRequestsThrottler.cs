using Coingecko.Api.Abstractions;
using Coingecko.Api.Extensions;
using Polly.CircuitBreaker;

namespace Coingecko.Api;

internal class GlobalApiRequestsThrottler : IGlobalApiRequestsThrottler
{
    private readonly CircuitBreakerAsync circuitBreakerAsync;

    public GlobalApiRequestsThrottler(CircuitBreakerAsync circuitBreakerAsync)
    {
        this.circuitBreakerAsync = circuitBreakerAsync;
    }  

    public async Task<T> Do<T>(Func<Task<T>> func, CancellationToken token)
    {
        try
        {
            return await func();
        }
        catch (BrokenCircuitException<HttpResponseMessage> ex)
            when (ex.Result.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            await circuitBreakerAsync.WaitUntilOpen(token);

            return await func();
        }
    }
}