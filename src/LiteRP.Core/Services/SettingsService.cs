using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LiteRP.Core.Models;
using LiteRP.Core.Services.Interfaces;

namespace LiteRP.Core.Services;

public class SettingsService : ISettingsService
{
    private static readonly string AppDataPath = 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LiteRP");

    private static readonly string SettingsFilePath = Path.Combine(AppDataPath, "settings.json");

    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    
    private AppSettings? _cachedSettings;
    
    public event Action? OnChange;

    public async Task<AppSettings> GetSettingsAsync()
    {
        if (_cachedSettings != null)
        {
            return _cachedSettings;
        }

        if (!File.Exists(SettingsFilePath))
        {
            _cachedSettings = new AppSettings();
            await SaveSettingsAsync(_cachedSettings);
            return _cachedSettings;
        }

        try
        {
            var json = await File.ReadAllTextAsync(SettingsFilePath);
            _cachedSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            return _cachedSettings;
        }
        catch (Exception)
        {
            _cachedSettings = new AppSettings();
            return _cachedSettings;
        }
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        Directory.CreateDirectory(AppDataPath);
        var json = JsonSerializer.Serialize(settings, SerializerOptions);
        await File.WriteAllTextAsync(SettingsFilePath, json);

        _cachedSettings = settings;

        OnChange?.Invoke();
    }
}