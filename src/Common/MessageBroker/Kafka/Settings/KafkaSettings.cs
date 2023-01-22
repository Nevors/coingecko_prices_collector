namespace Kafka.Settings;

public record KafkaSettings
{
    public string BootstrapServers { get; init; } = string.Empty;

    public EventsMap[] Events { get; init; } = Array.Empty<EventsMap>();

    public record EventsMap
    {
        public string TopicName { get; init; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
