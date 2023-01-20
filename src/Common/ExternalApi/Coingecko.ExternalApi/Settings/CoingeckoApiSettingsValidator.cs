using FluentValidation;

namespace Coingecko.Api.Settings;

public class CoingeckoApiSettingsValidator : AbstractValidator<CoingeckoApiSettings>
{
    public CoingeckoApiSettingsValidator()
    {
        RuleFor(s => s.BaseUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage(url => $"Value {url} is incorrect url");

        RuleFor(s => s.ProBaseUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage(url => $"Value {url} is incorrect url");
    }
}
