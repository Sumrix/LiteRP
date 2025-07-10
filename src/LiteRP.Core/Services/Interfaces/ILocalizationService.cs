using System.Collections.Generic;
using System.Globalization;

namespace LiteRP.Core.Services.Interfaces;

public interface ILocalizationService
{
    IEnumerable<CultureInfo> SupportedCultures { get; }
    CultureInfo CurrentCulture { get; }
    string GetDisplayName(CultureInfo culture);
    string GetFlagUrl(CultureInfo culture);
}