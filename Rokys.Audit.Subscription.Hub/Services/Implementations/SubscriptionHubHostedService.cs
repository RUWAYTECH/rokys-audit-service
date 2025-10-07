using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rokys.Audit.Subscription.Hub.Configuration;
using Rokys.Audit.Subscription.Hub.Services.Interfaces;

namespace Rokys.Audit.Subscription.Hub.Services.Implementations
{
    /// <summary>
    /// Servicio hospedado que maneja el ciclo de vida del Subscription Hub
    /// </summary>
    public class SubscriptionHubHostedService : BackgroundService
    {
        private readonly ISubscriptionHubService _subscriptionHubService;
        private readonly SubscriptionHubOptions _options;
        private readonly ILogger<SubscriptionHubHostedService> _logger;

        public SubscriptionHubHostedService(
            ISubscriptionHubService subscriptionHubService,
            IOptions<SubscriptionHubOptions> options,
            ILogger<SubscriptionHubHostedService> logger)
        {
            _subscriptionHubService = subscriptionHubService;
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Inicia el servicio en background
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (!_options.AutoStart)
                {
                    _logger.LogInformation("Subscription Hub auto-start is disabled");
                    return;
                }

                _logger.LogInformation("Starting Subscription Hub as background service...");

                // Iniciar el hub con timeout
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(_options.StartupTimeoutSeconds));

                await _subscriptionHubService.StartAsync(timeoutCts.Token);

                _logger.LogInformation("Subscription Hub background service started successfully");

                // Mantener el servicio corriendo hasta que se cancele
                try
                {
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Subscription Hub background service is shutting down...");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Subscription Hub background service");
                throw;
            }
        }

        /// <summary>
        /// Se ejecuta cuando el servicio se detiene
        /// </summary>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Stopping Subscription Hub background service...");

                if (_subscriptionHubService.IsRunning)
                {
                    await _subscriptionHubService.StopAsync(cancellationToken);
                }

                await base.StopAsync(cancellationToken);

                _logger.LogInformation("Subscription Hub background service stopped successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping Subscription Hub background service");
                throw;
            }
        }
    }
}