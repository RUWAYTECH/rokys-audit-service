using Ruway.Events.Command.Interfaces.Events;

namespace Rokys.Audit.Subscription.Hub.Services.Interfaces
{
    /// <summary>
    /// Servicio para manejar eventos de empleados
    /// </summary>
    public interface IUserEventService
    {
        /// <summary>
        /// Procesa evento de creación de empleado
        /// </summary>
        Task HandleUserCreatedAsync(UserCreatedEvent UserEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Procesa evento de actualización de empleado
        /// </summary>
        Task HandleUserUpdatedAsync(UserUpdatedEvent UserEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Procesa evento de eliminación de empleado
        /// </summary>
        Task HandleUserDeletedAsync(UserDeletedEvent UserEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Procesa mensaje genérico de empleado
        /// </summary>
        Task HandleGenericUserEventAsync(string message, string routingKey, CancellationToken cancellationToken = default);
    }
}