namespace Kafka.Settings;

public record KafkaSettings
{
    public string BootstrapServers { get; init; } = string.Empty;

    public TimeSpan ErrorTimeout { get; init; } = TimeSpan.FromSeconds(5);

    public EventSettings[] Events { get; init; } = Array.Empty<EventSettings>();

    public record EventSettings
    {
        public string TopicName { get; init; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string GroupId { get; set; } = string.Empty;
    }
}
