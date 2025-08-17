using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using LiteRP.Core.Enums;
using LiteRP.Core.Models;
using LiteRP.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OllamaSharp;
using OllamaSharp.Models;

namespace LiteRP.Core.Services;

public class OllamaStatusService : IDisposable
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<OllamaStatusService> _logger;
    private AppSettings _appSettings = new();
    private readonly OllamaOptions _ollamaOptions;
    private Timer? _timer;
    private readonly HashSet<object> _subscribers = [];

    public ConnectionStatus Status { get; private set; } = ConnectionStatus.Unknown;
    public event Action<ConnectionStatus>? StatusChanged;
    public IReadOnlyList<Model> Models = [];

    public OllamaStatusService(
        ISettingsService settingsService,
        IOptions<OllamaOptions> ollamaOptions,
        ILogger<OllamaStatusService> logger)
    {
        _settingsService = settingsService;
        _ollamaOptions = ollamaOptions.Value;
        _logger = logger;

        // Set initial connection status
        _ = TriggerCheckAsync();
    }

    public void StartMonitoring(object subscriber)
    {
        if (_subscribers.Add(subscriber) && _subscribers.Count == 1)
        {
            StartTimer();
        }
    }

    public void StopMonitoring(object subscriber)
    {
        if (_subscribers.Remove(subscriber) && _subscribers.Count == 0)
        {
            StopTimer();
        }
    }

    private void StartTimer()
    {
        if (_timer != null)
        {
            return;
        }

        _timer = new Timer(_ollamaOptions.ConnectionCheckIntervalSec * 1000);
        _timer.Elapsed += async (_, _) => await PerformCheckAsync();
        _timer.AutoReset = true;
        _timer.Start();
        _logger.LogInformation("Ollama status monitoring started.");
    }

    private void StopTimer()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        _logger.LogInformation("Ollama status monitoring stopped.");
    }

    public async Task TriggerCheckAsync()
    {
        _appSettings = await _settingsService.GetSettingsAsync();
        SetStatus(ConnectionStatus.Connecting);
        _ = PerformCheckAsync();
    }

    private async Task PerformCheckAsync()
    {
        if (Status != ConnectionStatus.Success)
            SetStatus(ConnectionStatus.Connecting);

        try
        {
            var uri = new Uri(_appSettings.OllamaUrl);
            var ollama = new OllamaApiClient(uri);
            Models = (await ollama.ListLocalModelsAsync()).ToArray();
            SetStatus(ConnectionStatus.Success);
        }
        catch (HttpRequestException e)
        {
            _logger.LogWarning("Couldn't connect to Ollama ({url}): '{message}'. Status Code: {status}.",
                _appSettings.OllamaUrl, e.Message, e.StatusCode);
            SetStatus(ConnectionStatus.Failed);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception during Ollama connection to {url}", _appSettings.OllamaUrl);
            SetStatus(ConnectionStatus.Failed);
        }
    }

    private void SetStatus(ConnectionStatus newStatus)
    {
        if (Status == newStatus)
        {
            return;
        }

        Status = newStatus;
        StatusChanged?.Invoke(newStatus);
    }

    public void Dispose()
    {
        StopTimer();
        GC.SuppressFinalize(this);
    }
}