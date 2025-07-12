using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LiteRP.Core.Models;
using LiteRP.Core.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace LiteRP.Core.Services;

public class LocalizationService : ILocalizationService
{
    // The dictionary that holds all our pre-calculated data.
    // It's private and readonly, an implementation detail.
    private readonly IReadOnlyDictionary<CultureInfo, CultureDisplayInfo> _cultureDisplayData;
    private readonly List<CultureInfo> _supportedCultures;

    public IEnumerable<CultureInfo> SupportedCultures => _supportedCultures;
    public CultureInfo CurrentCulture => CultureInfo.CurrentCulture;

    public LocalizationService(IOptions<LocalizationSettings> localizationOptions)
    {
        var settings = localizationOptions.Value;
        _supportedCultures = settings.SupportedCultures
            .Select(c => new CultureInfo(c))
            .ToList();
        
        var languageCounts = _supportedCultures
            .GroupBy(c => c.TwoLetterISOLanguageName)
            .ToDictionary(g => g.Key, g => g.Count());
        
        // Pre-calculate and store everything in the dictionary
        var displayData = new Dictionary<CultureInfo, CultureDisplayInfo>();
        foreach (var culture in _supportedCultures)
        {
            var displayName = CalculateDisplayName(culture, languageCounts);
            var flagUrl = CalculateFlagUrl(culture);
            displayData[culture] = new CultureDisplayInfo(displayName, flagUrl);
        }
        _cultureDisplayData = displayData;
    }

    // Public methods are now simple, fast lookups.
    public string GetDisplayName(CultureInfo culture) =>
        _cultureDisplayData.TryGetValue(culture, out var data) 
            ? data.DisplayName 
            : culture.Name; // Fallback for safety

    public string GetFlagUrl(CultureInfo culture) =>
        _cultureDisplayData.TryGetValue(culture, out var data) 
            ? data.FlagUrl 
            : string.Empty; // Fallback for safety

    private string CalculateDisplayName(CultureInfo culture, IReadOnlyDictionary<string, int> langCounts)
    {
        var languageName = char.ToUpperInvariant(culture.NativeName[0]) + culture.NativeName.Substring(1);

        if (langCounts.TryGetValue(culture.TwoLetterISOLanguageName, out int count) && count == 1)
        {
            if (languageName.Contains('('))
            {
                languageName = languageName.Substring(0, languageName.IndexOf('(')).Trim();
            }
            return languageName;
        }

        var countryCode = GetCountryCode(culture.Name).ToUpper();
        return $"{culture.Parent.NativeName} ({countryCode})";
    }

    private string CalculateFlagUrl(CultureInfo culture)
    {
        var countryCode = GetCountryCode(culture.Name);
        return $"img/{countryCode}.svg";
    }
    
    private string GetCountryCode(string cultureName)
    {
        if (cultureName.Equals("en", StringComparison.OrdinalIgnoreCase)) return "us";
        if (cultureName.Contains('-')) return cultureName.Split('-')[1].ToLowerInvariant();
        return cultureName.ToLowerInvariant();
    }
}