using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace Coingecko.Api.Extensions;

internal record CircuitBreakerAsync
{
    public AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreaker { get; }

    private TaskCompletionSource? waitUntilOpen;

    public CircuitBreakerAsync(ILogger<CircuitBreakerAsync> logger)
    {
        CircuitBreaker = Policy
            .HandleResult<HttpResponseMessage>(
                    r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .CircuitBreakerAsync(1, TimeSpan.FromSeconds(120),
                (result, ts) =>
                {
                    waitUntilOpen = new TaskCompletionSource();

                    logger.LogInformation("Блокировка доступа к апи на {ts}.", ts);

                    Task.Delay(ts).ContinueWith(_ =>
                    {
                        CircuitBreaker!.Reset();

                        waitUntilOpen.SetResult();
                        waitUntilOpen = null;
                    });
                },
                onReset: () => logger.LogInformation("Доступ восстановлен"));
    }

    public Task WaitUntilOpen(CancellationToken token)
    {
        return waitUntilOpen == null
            ? Task.CompletedTask
            : waitUntilOpen.Task.WaitAsync(token);
    }
}
