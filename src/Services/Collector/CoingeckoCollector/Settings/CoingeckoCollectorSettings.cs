namespace CoingeckoCollector.Settings;

public class CoingeckoCollectorSettings
{
    public string[] Currencies { get; init; } = Array.Empty<string>();

    public bool UsePro { get; init; } = true;

    public TimeSpan WorkerInterval { get; set; } = TimeSpan.FromSeconds(60);
}
