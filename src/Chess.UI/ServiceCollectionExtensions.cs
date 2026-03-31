using Chess.UI.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Chess.UI;

internal static class ServiceCollectionExtensions
{
    public static void AddChessLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(ConfigureSerilog);
        void ConfigureSerilog(ILoggingBuilder loggingBuilder)
        {
            var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(logger);
            });
        }
    }

    public static void AddChessServices(this IServiceCollection services)
    {
        services.AddTransient<MainViewModel>();
    }
}
