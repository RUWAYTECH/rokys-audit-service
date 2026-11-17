using Ruway.Events.Command.Interfaces.Events;

namespace Rokys.Audit.Subscription.Hub.Services.Interfaces
{
    public interface IEnterpriseEventService
    {
        /// <summary>
        /// Procesa evento de creación de la empresa
        /// </summary>
        Task HandleEnterpriseCreatedAsync(EnterpriseCreatedEvent StoreEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Procesa evento de actualización de la empresa
        /// </summary>
        Task HandleEnterpriseUpdatedAsync(EnterpriseUpdatedEvent StoreEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Procesa evento de eliminación de la empresa
        /// </summary>
        Task HandleEnterpriseDeletedAsync(EnterpriseDeletedEvent StoreEvent, CancellationToken cancellationToken = default);
    }
}
