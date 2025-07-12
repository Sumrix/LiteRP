namespace LiteRP.Core.Models;

public class AppSettings
{
    public string OllamaUrl { get; set; } = "http://localhost:11434";
    public string ModelName { get; set; } = string.Empty;
    public AppTheme Theme { get; set; } = AppTheme.Dark;
    public string AccentColor { get; set; } = "#FF6C00";
}