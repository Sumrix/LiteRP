using System.Globalization;
using LiteRP.Core.Models;
using LiteRP.Core.Services;
using LiteRP.Core.Services.Interfaces;
using Microsoft.AspNetCore.Localization;

namespace LiteRP.WebApp.Helpers;

public static class ServiceCollectionHelper
{
    public static IServiceCollection AddLocalizationServices(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.Configure<LocalizationSettings>(
            configuration.GetSection("LocalizationSettings"));
    
        services.AddSingleton<ILocalizationService, LocalizationService>();
    
        var localizationSettings = configuration.GetSection("LocalizationSettings").Get<LocalizationSettings>()
                                   ?? new LocalizationSettings();

        var supportedCultures = localizationSettings.SupportedCultures
            .Select(c => new CultureInfo(c)).ToList();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(localizationSettings.DefaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        return services;
    }
}