using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace LiteRP.WebApp.Services;

public class BrowserLauncherHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _life;
    private readonly IServerAddressesFeature? _addrs;
    public BrowserLauncherHostedService(
        IHostApplicationLifetime life,
        IServer server)
    {
        _life  = life;
        _addrs = server.Features.Get<IServerAddressesFeature>();
    }

    public Task StartAsync(CancellationToken _) {
        _life.ApplicationStarted.Register(() =>
            LaunchBrowser(_addrs?.Addresses.First() ??
                          "http://localhost:5000"));
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