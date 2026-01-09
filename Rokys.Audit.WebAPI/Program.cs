using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using System.IO;

namespace Rokys.Audit.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configurar Serilog antes de construir el host
            ConfigureSerilog();
            
            BuildWebHost(args).Run();
        }

        private static void ConfigureSerilog()
        {
            // Crear directorio de logs si no existe
            var logDirectory = Path.Combine(AppContext.BaseDirectory, "LogError");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var logFilePath = Path.Combine(logDirectory, "log-.txt");
            var subscriptionLogPath = Path.Combine(logDirectory, "subscription-trace-.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                // Log general en archivo principal
                .WriteTo.File(logFilePath, 
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                // Log específico para todos los eventos de suscripción
                .WriteTo.File(subscriptionLogPath,
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
                
            Log.Information("========================================");
            Log.Information("Serilog configured successfully");
            Log.Information("Application base directory: {BaseDirectory}", AppContext.BaseDirectory);
            Log.Information("Log directory: {LogDirectory}", logDirectory);
            Log.Information("Main log file: {LogFile}", logFilePath);
            Log.Information("Subscription trace log: {SubscriptionLog}", subscriptionLogPath);
            Log.Information("========================================");
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}