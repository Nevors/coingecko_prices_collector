namespace Coingecko.Api.Abstractions.Queries;

public interface ISuppotedCoinsQuery
{
    Task<int> GetCountAsync(CancellationToken token);
}
