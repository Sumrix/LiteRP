using Flowbite.Services;
using LiteRP.Core.Helpers;
using LiteRP.Core.Models;
using LiteRP.Core.Services;
using LiteRP.Core.Services.Interfaces;
using LiteRP.WebApp.Components;
using LiteRP.WebApp.Helpers;
using LiteRP.WebApp.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting LiteRP");

    var builder = WebApplication.CreateBuilder(args);
    
    builder.Services.AddSerilog(lc => 
        lc.ReadFrom.Configuration(builder.Configuration)
            .WriteTo.File(
                Path.Combine(PathManager.LogsPath, "log-.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                shared: true));

    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddLocalizationServices(builder.Configuration);

    builder.Services.AddControllers(); 

    builder.Services.AddFlowbite();

    builder.Services.Configure<AvatarSettings>(builder.Configuration.GetSection("AvatarSettings"));

    builder.Services.AddKernel();

    builder.Services.AddHttpClient();

    builder.Services.AddSingleton<ISettingsService, SettingsService>();
    builder.Services.AddSingleton<ICharacterService, CharacterService>();
    builder.Services.AddSingleton<ILorebookService, LorebookService>();
    builder.Services.AddSingleton<IAvatarService, AvatarService>();
    builder.Services.AddSingleton<IChatSessionService, ChatSessionService>();

    builder.Services.AddScoped<IElementClickObserverService, ElementClickObserverService>();

    builder.Services.AddOutputCache(options =>
    {
        options.AddPolicy("AvatarPolicy", policy =>
            policy.SetVaryByQuery("v", "dpr").Expire(TimeSpan.FromDays(365)));
    });

    builder.Services.AddLocalization();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
    }

    app.UseRequestLocalization();

    app.UseStatusCodePagesWithReExecute("/not-found");

    app.UseOutputCache();

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapControllers();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

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