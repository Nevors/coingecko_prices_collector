using Microsoft.Extensions.DependencyInjection;
using Coingecko.Api.Settings;
using Coingecko.Api.Abstractions;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Coingecko.Api.Contracts;
using Coingecko.Api.Abstractions.Queries;
using Coingecko.Api.Queries;
using RestSharp;
using Microsoft.Extensions.Logging;
using Polly.Extensions.Http;
using Coingecko.Api.ApiClient;
using Microsoft.Extensions.Configuration;
using Configuration.Extensions;

namespace Coingecko.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoingeckoApi(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.ConfigureSettings<CoingeckoApiSettings, CoingeckoApiSettingsValidator>(configuration);

        services.AddSingleton<CircuitBreakerAsync>();

        services.AddPolicyRegistry((sp, registry) =>
        {
            var throttler = sp.GetRequiredService<IGlobalApiRequestsThrottler>();
            var circuitBreaker = sp.GetRequiredService<CircuitBreakerAsync>();

            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Coingecko.Api");

            var commonPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5));

            var tooManyRequestsPolicy = Policy
                .HandleResult<HttpResponseMessage>(
                    r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(300), 4));

            registry.Add(nameof(RestClient), Policy.WrapAsync(circuitBreaker.CircuitBreaker, tooManyRequestsPolicy, commonPolicy));
        });

        services
            .AddHttpClient<RestClient, RestClient>((httpClient, restClient) =>
            {
                var client = new RestClient(httpClient);

                client.Options.ThrowOnAnyError = true;

                if (string.IsNullOrEmpty(settings.ApiKey))
                {
                    client.Options.BaseUrl = new Uri(settings.BaseUrl);
                }
                else
                {
                    client.AddDefaultQueryParameter("x_cg_pro_api_key", settings.ApiKey);
                    client.Options.BaseUrl = new Uri(settings.ProBaseUrl);
                }

                return client;
            })
            .AddPolicyHandlerFromRegistry(nameof(RestClient));

        services.AddSingleton<IGlobalApiRequestsThrottler, GlobalApiRequestsThrottler>();

        services.AddSingleton(typeof(IParallelActionsThrottler<>), typeof(ParallelActionsThrottler<>));
        services.AddSingleton(typeof(IGlobalMemmoryCache<>), typeof(GlobalCache<>));

        services.AddTransient<ICurrencyCoinsPageQuery, CurrencyCoinsPageQuery>();
        if (settings.SuppressLoadingCoinsPageErrors)
        {
            services.Decorate<ICurrencyCoinsPageQuery, CurrencyCoinsPageQuerySuppressDecorator>();
        }

        services.AddTransient<IApiClient<ICurrencyCoinsPageQuery>, Client<ICurrencyCoinsPageQuery>>();
        services.Decorate<IApiClient<ICurrencyCoinsPageQuery>, ClientBulkheadIsolationDecorator<ICurrencyCoinsPageQuery>>();

        services.AddTransient<ISuppotedCoinsQuery, SuppotedCoinsQuery>();

        services.AddTransient<IApiClient<ISuppotedCoinsQuery>, Client<ISuppotedCoinsQuery>>();
        services.Decorate<IApiClient<ISuppotedCoinsQuery>, ClientCacheDecorator<ISuppotedCoinsQuery>>();

        if (settings.WaitInCaseOfTooManyRequests)
        {
            services.Decorate(typeof(IApiClient<>), typeof(ClientApiRequestsThrottleDecorator<>));
        }

        services.AddTransient<ICoingeckoCoinsApi, CoingeckoCoinsApi>();

        return services;
    }
}
