using Ruway.Events.Command.Interfaces.Events;

namespace Rokys.Audit.Subscription.Hub.Services.Interfaces
{
    /// <summary>
    /// Servicio para manejar eventos de empleados
    /// </summary>
    public interface IStoreEventService
    {
        /// <summary>
        /// Procesa evento de creación de empleado
        /// </summary>
        Task HandleStoreCreatedAsync(StoreCreatedEvent StoreEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Procesa evento de actualización de empleado
        /// </summary>
        Task HandleStoreUpdatedAsync(StoreUpdatedEvent StoreEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Procesa evento de eliminación de empleado
        /// </summary>
        Task HandleStoreDeletedAsync(StoreDeletedEvent StoreEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Procesa mensaje genérico de empleado
        /// </summary>
        Task HandleGenericStoreEventAsync(string message, string routingKey, CancellationToken cancellationToken = default);
    }
}