namespace Rokys.Audit.Subscription.Hub.Services.Interfaces
{
    /// <summary>
    /// Servicio principal del hub de suscripciones
    /// </summary>
    public interface ISubscriptionHubService
    {
        /// <summary>
        /// Inicia el hub de suscripciones
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Detiene el hub de suscripciones
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Indica si el hub está activo
        /// </summary>
        bool IsRunning { get; }
    }
}