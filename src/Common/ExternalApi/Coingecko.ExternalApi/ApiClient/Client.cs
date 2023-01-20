using Coingecko.Api.Abstractions;
using RestSharp;

namespace Coingecko.Api.ApiClient;

internal class Client<TMarker> : IApiClient<TMarker>
{
    private readonly RestClient restClient;

    public Client(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public Task<T> Execute<T>(Func<RestClient, CancellationToken, Task<T>> execute, CancellationToken token)
        => execute(restClient, token);
}
