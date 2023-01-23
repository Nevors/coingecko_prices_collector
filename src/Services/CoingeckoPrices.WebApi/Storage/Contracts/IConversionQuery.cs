namespace CoingeckoPrices.WebApi.Storage.Contracts;

public interface IConversionQuery
{
    Task<double> ComputeConversion(string baseCoin, string quoteCoin, CancellationToken token);
}

public class PairNotFoundException : Exception
{
    public PairNotFoundException(string @base, string quote)
        : base($"Пара {@base}_{quote} не найдена")
    {
        Base = @base;
        Quote = quote;
    }

    public string Base { get; }

    public string Quote { get; }
}