using Coingecko.Api.Abstractions;
using Coingecko.Api.Abstractions.Queries;
using RestSharp;

namespace Coingecko.Api.ApiClient;

internal class ClientCacheDecorator<TMarker> : IApiClient<TMarker>
{
    private readonly IApiClient<TMarker> inner;
    private readonly IGlobalMemmoryCache<ISuppotedCoinsQuery> cache;

    public ClientCacheDecorator(
        IApiClient<TMarker> inner,
        IGlobalMemmoryCache<ISuppotedCoinsQuery> cache)
    {
        this.inner = inner;
        this.cache = cache;
    }

    public Task<T> Execute<T>(Func<RestClient, CancellationToken, Task<T>> execute, CancellationToken token)
        => cache.Get(() => inner.Execute(execute, token), token);
}