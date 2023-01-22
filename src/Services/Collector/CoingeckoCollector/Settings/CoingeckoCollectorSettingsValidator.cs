using FluentValidation;

namespace CoingeckoCollector.Settings;

public class CoingeckoCollectorSettingsValidator : AbstractValidator<CoingeckoCollectorSettings>
{
	public CoingeckoCollectorSettingsValidator()
	{
		RuleFor(s => s.Currencies)
			.NotEmpty();
    }
}
