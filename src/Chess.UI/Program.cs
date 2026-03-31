using Avalonia;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Chess.UI;

static class Program
{
    public static IServiceProvider Services { get; private set; } = null!;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Build configuration
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        // Setup Serilog FIRST
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .CreateLogger();

        try
        {
            Log.Information("Starting application");

            // Setup DI
            var services = new ServiceCollection();
            services.AddChessLogging(config);
            services.AddChessServices();

            Services = services.BuildServiceProvider();

            // Start Avalonia
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
#pragma warning disable CA1031
        catch (Exception ex)
#pragma warning restore CA1031
        {
            Log.Fatal(ex, "Application failed to start");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
