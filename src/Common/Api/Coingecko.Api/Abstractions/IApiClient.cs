using RestSharp;

namespace Coingecko.Api.Abstractions;

public interface IApiClient<TMarker>
{
    Task<T> Execute<T>(Func<RestClient, CancellationToken, Task<T>> execute, CancellationToken token);
}
