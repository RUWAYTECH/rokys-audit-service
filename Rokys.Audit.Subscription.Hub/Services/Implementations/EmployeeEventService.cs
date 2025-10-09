using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rokys.Audit.Subscription.Hub.Services.Interfaces;
using Ruway.Events.Command.Interfaces.Events;

namespace Rokys.Audit.Subscription.Hub.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de manejo de eventos de empleados
    /// </summary>
    public class EmployeeEventService : IEmployeeEventService
    {
        private readonly ILogger<EmployeeEventService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EmployeeEventService(
            ILogger<EmployeeEventService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public async Task HandleEmployeeCreatedAsync(EmployeeCreatedEvent employeeEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Processing employee created event for Employee ID: {EmployeeId}, Name: {FirstName} {LastName}",
                    employeeEvent.EmployeeId, employeeEvent.FirstName, employeeEvent.LastName);

                // Aquí puedes agregar lógica específica para cuando se crea un empleado
                // Por ejemplo: enviar notificaciones de bienvenida, crear perfiles de usuario, etc.

                // Ejemplo de uso del IEmployeeService si necesitas validar o obtener más información
                // var employeeService = _serviceProvider.GetRequiredService<IEmployeeService>();
                // var employeeDetails = await employeeService.GetByIdAsync(employeeEvent.EmployeeId);

                var fullName = $"{employeeEvent.FirstName} {employeeEvent.LastName}";
                await LogEmployeeActivity("CREATED", employeeEvent.EmployeeId, fullName,
                    $"Employee created - Document: {employeeEvent.DocumentNumber}, Email: {employeeEvent.Email}, Phone: {employeeEvent.Phone}");

                _logger.LogInformation("Successfully processed employee created event for Employee ID: {EmployeeId}", 
                    employeeEvent.EmployeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing employee created event for Employee ID: {EmployeeId}", 
                    employeeEvent.EmployeeId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task HandleEmployeeUpdatedAsync(EventWrapper<EmployeeUpdatedEvent> employeeEvent, CancellationToken cancellationToken = default)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing employee updated event for Employee ID: {EmployeeId}", 
                    employeeEvent.Data.EmployeeId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task HandleEmployeeDeletedAsync(EventWrapper<EmployeeDeletedEvent> employeeEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Processing employee deleted event for Employee ID: {EmployeeId}",
                    employeeEvent.Data.EmployeeId);

                await LogEmployeeActivity("DELETED", employeeEvent.Data.EmployeeId, "N/A",
                    $"Employee deleted - Employee ID: {employeeEvent.Data.EmployeeId}");

                // Aquí puedes agregar lógica adicional como:
                // - Revocar accesos
                // - Enviar notificaciones de despedida
                // - Transferir responsabilidades
                // - Archivar datos

                _logger.LogInformation("Successfully processed employee deleted event for Employee ID: {EmployeeId}", 
                    employeeEvent.Data.EmployeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing employee deleted event for Employee ID: {EmployeeId}", 
                    employeeEvent.Data.EmployeeId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task HandleGenericEmployeeEventAsync(string message, string routingKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Processing generic employee event with routing key: {RoutingKey}", routingKey);

                // Intentar deserializar como diferentes tipos de eventos
                await TryProcessAsTypedEvent(message, routingKey, cancellationToken);

                _logger.LogDebug("Successfully processed generic employee event with routing key: {RoutingKey}", routingKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing generic employee event with routing key: {RoutingKey}. Message: {Message}", 
                    routingKey, message);
                throw;
            }
        }

        /// <summary>
        /// Intenta procesar un mensaje como evento tipado
        /// </summary>
        private async Task TryProcessAsTypedEvent(string message, string routingKey, CancellationToken cancellationToken = default)
        {
            try
            {
                // Determinar tipo de evento basado en routing key
                switch (routingKey.ToLower())
                {
                    case var key when key.Contains("employee.events.created"):
                        var createdEvent = JsonConvert.DeserializeObject<EventWrapper<EmployeeCreatedEvent>>(message);
                        if (createdEvent != null)
                            await HandleEmployeeCreatedAsync(createdEvent.Data, cancellationToken);
                        break;

                    case var key when key.Contains("employee.events.updated"):
                        var updatedEvent = JsonConvert.DeserializeObject<EventWrapper<EmployeeUpdatedEvent>>(message);
                        if (updatedEvent != null)
                            await HandleEmployeeUpdatedAsync(updatedEvent, cancellationToken);
                        break;

                    case var key when key.Contains("employee.events.deleted"):
                        var deletedEvent = JsonConvert.DeserializeObject<EventWrapper<EmployeeDeletedEvent>>(message);
                        if (deletedEvent != null)
                            await HandleEmployeeDeletedAsync(deletedEvent, cancellationToken);
                        break;

                    default:
                        _logger.LogWarning("Unknown employee event routing key: {RoutingKey}", routingKey);
                        break;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize employee event message: {Message}", message);
            }
        }

        /// <summary>
        /// Registra la actividad del empleado (puedes implementar persistencia aquí)
        /// </summary>
        private async Task LogEmployeeActivity(string action, Guid employeeId, string? employeeName, string description)
        {
            // Aquí puedes implementar el logging persistente
            // Por ejemplo, guardar en base de datos, enviar a un servicio de auditoría, etc.
            
            _logger.LogInformation("Employee Activity - Action: {Action}, EmployeeId: {EmployeeId}, Name: {EmployeeName}, Description: {Description}",
                action, employeeId, employeeName, description);

            // Ejemplo de como podrías usar un servicio de auditoría si existe
            // var auditService = _serviceProvider.GetService<IAuditService>();
            // if (auditService != null)
            // {
            //     await auditService.LogActivityAsync(new AuditEntry
            //     {
            //         EntityType = "Employee",
            //         EntityId = employeeId.ToString(),
            //         Action = action,
            //         Description = description,
            //         Timestamp = DateTime.UtcNow
            //     });
            // }

            await Task.CompletedTask;
        }
    }
}