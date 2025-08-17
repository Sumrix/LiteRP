using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LiteRP.Core.Helpers;
using LiteRP.Core.Models;
using LiteRP.Core.Services.Interfaces;

namespace LiteRP.Core.Services;

public class SettingsService : ISettingsService
{
    private AppSettings? _cachedSettings;
    
    public event Action? OnChange;

    public async Task<AppSettings> GetSettingsAsync()
    {
        if (_cachedSettings != null)
        {
            return _cachedSettings.Clone();
        }

        if (!File.Exists(PathManager.SettingsFilePath))
        {
            _cachedSettings = new AppSettings();
            await SaveSettingsAsync(_cachedSettings);
            return _cachedSettings.Clone();
        }

        try
        {
            var json = await File.ReadAllTextAsync(PathManager.SettingsFilePath);
            _cachedSettings = JsonSerializer.Deserialize<AppSettings>(json, JsonHelper.Context.AppSettings) ?? new AppSettings();
            return _cachedSettings.Clone();
        }
        catch (Exception)
        {
            _cachedSettings = new AppSettings();
            return _cachedSettings.Clone();
        }
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, JsonHelper.Context.AppSettings);
        await File.WriteAllTextAsync(PathManager.SettingsFilePath, json);

        _cachedSettings = settings;

        OnChange?.Invoke();
    }
}