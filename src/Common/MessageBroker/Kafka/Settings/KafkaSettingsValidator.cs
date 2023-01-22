using FluentValidation;

namespace Kafka.Settings;

public class KafkaSettingsValidator : AbstractValidator<KafkaSettings>
{
    public KafkaSettingsValidator()
    {
        RuleFor(s => s.BootstrapServers)
            .NotEmpty();
    }
}
