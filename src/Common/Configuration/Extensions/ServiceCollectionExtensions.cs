using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Configuration.Extensions;

public static class ServiceCollectionExtensions
{
    public static TSetting ConfigureSettings<TSetting>(this IServiceCollection services, IConfiguration configuration, string? configurationSectionName = null)
        where TSetting : class, new()
    {
        var section = configuration.GetSection(configurationSectionName ?? typeof(TSetting).Name);

        services.Configure<TSetting>(section);
        services.AddTransient(sp => sp.GetRequiredService<IOptions<TSetting>>().Value);

        return section.Get<TSetting>();
    }

    public static TSetting ConfigureSettings<TSetting, TValidator>(this IServiceCollection services, IConfiguration configuration, string? configurationSectionName = null)
            where TSetting : class, new()
            where TValidator : AbstractValidator<TSetting>, new()
    {
        var settingInstance = services.ConfigureSettings<TSetting>(configuration, configurationSectionName);

        if (settingInstance is null)
        {
            throw new InvalidOperationException("Конфигурируемые настройки не найдены");
        }

        new TValidator().ValidateAndThrow(settingInstance);

        return settingInstance;
    }
}
