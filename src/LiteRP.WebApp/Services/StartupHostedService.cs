using System.Diagnostics;
using System.Runtime.InteropServices;
using LiteRP.Core.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace LiteRP.WebApp.Services;

public class StartupHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _life;
    private readonly IServiceProvider _services;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IServerAddressesFeature? _addresses;

    public StartupHostedService(
        IHostApplicationLifetime life,
        IServer server,
        IServiceProvider services,
        IHostEnvironment hostEnvironment)
    {
        _life  = life;
        _services = services;
        _hostEnvironment = hostEnvironment;
        _addresses = server.Features.Get<IServerAddressesFeature>();
    }

    public Task StartAsync(CancellationToken _)
    {
        if (_hostEnvironment.IsProduction())
        {
            _life.ApplicationStarted.Register(() =>
                LaunchBrowser(_addresses?.Addresses.First() ?? "http://localhost:5000"));
        }

        _services.GetService<OllamaStatusService>();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken _) => Task.CompletedTask;

    private static void LaunchBrowser(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
    }
}