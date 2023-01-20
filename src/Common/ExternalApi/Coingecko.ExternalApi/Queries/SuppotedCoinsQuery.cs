using Coingecko.Api.Abstractions;
using Coingecko.Api.Abstractions.Queries;
using RestSharp;

namespace Coingecko.Api.Queries;

internal class SuppotedCoinsQuery : ISuppotedCoinsQuery
{
    private readonly IApiClient<ISuppotedCoinsQuery> apiClient;

    public SuppotedCoinsQuery(IApiClient<ISuppotedCoinsQuery> apiClient)
    {
        this.apiClient = apiClient;
    }

    public Task<int> GetCountAsync(CancellationToken token)
    {
        return apiClient.Execute(
            async (rest, token) =>
            {
                var coins = await rest.GetAsync<object[]>(new("/coins/list"), token);

                return coins!.Length;
            },
            token);
    }
}
