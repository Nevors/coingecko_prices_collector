using Coingecko.Api.Abstractions;
using Coingecko.Api.Abstractions.Queries;
using Coingecko.Api.Contracts.Dto.Coins;
using RestSharp;
using System.Text.Json.Serialization;

namespace Coingecko.Api.Queries;

internal class CurrencyCoinsPageQuery : ICurrencyCoinsPageQuery
{
    private readonly IApiClient<ICurrencyCoinsPageQuery> apiClient;

    public CurrencyCoinsPageQuery(IApiClient<ICurrencyCoinsPageQuery> apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IEnumerable<CoinDto>> GetCoinsPageAsync(ICurrencyCoinsPageQuery.Request request, CancellationToken token)
    {
        var response = await apiClient.Execute(
            (rest, token) => rest.GetAsync<IEnumerable<ResponseItem>>(
                new RestRequest("/coins/markets")
                    .AddQueryParameter("vs_currency", request.Currency)
                    .AddQueryParameter("per_page", request.PageSize)
                    .AddQueryParameter("page", request.PageNumber), token),
            token);

        return response!.Select(_ => new CoinDto { Symbol = _.Symbol, CurrentPrice = _.CurrentPrice });
    }

    record ResponseItem
    {
        public string Symbol { get; init; } = string.Empty;

        [JsonPropertyName("current_price")]
        public double? CurrentPrice { get; init; }
    }
}


