using Coingecko.Api.Abstractions;
using RestSharp;

namespace Coingecko.Api.ApiClient;

internal class ClientApiRequestsThrottleDecorator<TMarker> : IApiClient<TMarker>
{
    private readonly IApiClient<TMarker> inner;
    private readonly IGlobalApiRequestsThrottler throttleAction;

    public ClientApiRequestsThrottleDecorator(
        IApiClient<TMarker> inner,
        IGlobalApiRequestsThrottler throttleAction)
    {
        this.inner = inner;
        this.throttleAction = throttleAction;
    }

    public Task<T> Execute<T>(Func<RestClient, CancellationToken, Task<T>> execute, CancellationToken token)
        => throttleAction.Do(() => inner.Execute(execute, token), token);
}
