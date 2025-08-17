using System.Reflection;
using LiteRP.Core.Helpers;
using LiteRP.Core.Models;
using LiteRP.Core.Services;
using LiteRP.Core.Services.Interfaces;
using LiteRP.WebApp.Components;
using LiteRP.WebApp.Components.Flowbite;
using LiteRP.WebApp.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var assembly = Assembly.GetExecutingAssembly();

    IConfiguration embeddedConfig;
    await using (var cfgStream = assembly.GetManifestResourceStream("LiteRP.WebApp.appsettings.json"))
    {
        if (cfgStream is null)
            throw new InvalidOperationException("Embedded appsettings.json not found.");

        embeddedConfig = new ConfigurationBuilder()
            .AddJsonStream(cfgStream)
            .AddEnvironmentVariables()
            .Build();
    }

    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.Sources.Clear();
    builder.Configuration.AddConfiguration(embeddedConfig);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.File(
            Path.Combine(PathManager.LogsPath, "log-.txt"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14,
            shared: true)
        .CreateLogger();

    Log.Information("------------Starting LiteRP------------");
    
    builder.Services.AddSerilog(Log.Logger);
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents()
        .AddCircuitOptions(options => options.DetailedErrors = true);
    builder.Services.AddFlowbite();
    builder.Services.Configure<AvatarOptions>(builder.Configuration.GetSection("Avatar"));
    builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection("Ollama"));
    builder.Services.AddKernel();
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<ISettingsService, SettingsService>();
    builder.Services.AddSingleton<ICharacterService, CharacterService>();
    builder.Services.AddSingleton<ILorebookService, LorebookService>();
    builder.Services.AddSingleton<IAvatarService, AvatarService>();
    builder.Services.AddSingleton<IChatSessionStateService, ChatSessionStateService>();
    // ChatSessionService is scoped, because each scope must have different ChatSessionService.NewSession instance
    builder.Services.AddScoped<IChatSessionService, ChatSessionService>();
    builder.Services.AddSingleton<OllamaStatusService>();
    builder.Services.AddScoped<IElementClickObserverService, ElementClickObserverService>();
    builder.Services.AddOutputCache(options =>
    {
        options.AddPolicy("AvatarPolicy", policy =>
            policy.SetVaryByQuery("v", "dpr").Expire(TimeSpan.FromDays(365)));
    });
    builder.Services.AddHostedService<StartupHostedService>();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
    }
    app.UseStatusCodePagesWithReExecute("/not-found");
    app.UseOutputCache();
    app.UseAntiforgery();

    var provider = new ManifestEmbeddedFileProvider(assembly, "wwwroot");
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = provider
    });

    var avatars = app.MapGroup("/characters/{characterId:guid}/avatar")
        .WithGroupName("avatars");

    // GET /characters/{id}/avatar/{sizeToken}?v=...&dpr=...
    avatars.MapGet("/{sizeToken}", async (
            Guid characterId,
            string sizeToken,
            int? v,
            int dpr,
            IAvatarService avatarService,
            HttpResponse http) =>
        {
            if (dpr <= 0) dpr = 1;

            var stream = await avatarService.GetResizedAvatarStreamAsync(characterId, sizeToken, dpr);
            if (stream is null)
                return Results.NotFound();

            const long oneYear = 31536000;
            http.Headers.CacheControl = $"public,max-age={oneYear},immutable";
            http.Headers[HeaderNames.ContentType] = "image/webp";

            return Results.Stream(stream, "image/webp");
        })
        .CacheOutput("AvatarPolicy");

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();
    
    Log.Information("Initialization complete");

    await app.RunAsync();
    
    Log.Information("Stopped cleanly");
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}