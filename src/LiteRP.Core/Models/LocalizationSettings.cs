namespace LiteRP.Core.Models;

public class LocalizationSettings
{
    public string DefaultCulture { get; set; } = "en-US";
    public string[] SupportedCultures { get; set; } = [];
}