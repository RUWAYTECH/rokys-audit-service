using Ruway.Events.Command.Interfaces.Events;

namespace Rokys.Audit.Subscription.Hub.Services.Interfaces
{
    /// <summary>
    /// Servicio para manejar eventos de empleados
    /// </summary>
    public interface IEmployeeEventService
    {
        /// <summary>
        /// Procesa evento de creación de empleado
        /// </summary>
        Task HandleEmployeeCreatedAsync(EmployeeCreatedEvent employeeEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Procesa evento de actualización de empleado
        /// </summary>
        Task HandleEmployeeUpdatedAsync(EventWrapper<EmployeeUpdatedEvent> employeeEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Procesa evento de eliminación de empleado
        /// </summary>
        Task HandleEmployeeDeletedAsync(EventWrapper<EmployeeDeletedEvent> employeeEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Procesa mensaje genérico de empleado
        /// </summary>
        Task HandleGenericEmployeeEventAsync(string message, string routingKey, CancellationToken cancellationToken = default);
    }
}