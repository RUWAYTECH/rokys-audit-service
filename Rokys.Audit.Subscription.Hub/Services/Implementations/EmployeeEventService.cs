using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rokys.Audit.DTOs.Requests.EmployeeStore;
using Rokys.Audit.Services.Interfaces;
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
        private readonly IUserReferenceService _userReferenceService;

        public EmployeeEventService(
            ILogger<EmployeeEventService> logger,
            IUserReferenceService userReferenceService)
        {
            _logger = logger;
            _userReferenceService = userReferenceService;
        }

        /// <inheritdoc />
        public async Task HandleEmployeeCreatedAsync(EmployeeCreatedEvent employeeEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SUBSCRIPTION-TRACE] EmployeeCreated event received - EmployeeId: {EmployeeId}, Name: {FirstName} {LastName}, EventId: {EventId}", 
                employeeEvent.EmployeeId, employeeEvent.FirstName, employeeEvent.LastName, employeeEvent.EventId);
            
            try
            {
               await _userReferenceService.Create(new DTOs.Requests.UserReference.UserReferenceRequestDto
                {
                    UserId = null,
                    EmployeeId = employeeEvent.EmployeeId,
                    FirstName = employeeEvent.FirstName,
                    LastName = employeeEvent.LastName,
                    Email = employeeEvent.Email,
                    PersonalEmail = employeeEvent.PersonalEmail,
                    DocumentNumber = employeeEvent.DocumentNumber,
                    RoleCode = null,
                    RoleName = null,
                    EmployeeStores = employeeEvent.EmployeeStores
                        .Select(es => new DTOs.Requests.UserReference.EmployeeStoreReferenceRequestDto
                        {
                            StoreId = es.StoreId,
                            AssignmentDate = es.AssignmentDate
                        })
                        .ToArray(),
               });

                _logger.LogInformation("[SUBSCRIPTION-TRACE] EmployeeCreated processed successfully - EmployeeId: {EmployeeId}", employeeEvent.EmployeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SUBSCRIPTION-ERROR] Error processing EmployeeCreated - EmployeeId: {EmployeeId}", employeeEvent.EmployeeId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task HandleEmployeeUpdatedAsync(EmployeeUpdatedEvent employeeEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SUBSCRIPTION-TRACE] EmployeeUpdated event received - EmployeeId: {EmployeeId}, Name: {FirstName} {LastName}, EventId: {EventId}", 
                employeeEvent.EmployeeId, employeeEvent.FirstName, employeeEvent.LastName, employeeEvent.EventId);
            
            try
            {
                var userReference = await _userReferenceService.GetByEmployeeId(employeeEvent.EmployeeId);
                if (userReference.Data != null)
                {
                    var userRef = userReference.Data;
                    await _userReferenceService.Update(userRef.UserReferenceId, new DTOs.Requests.UserReference.UserReferenceRequestDto
                    {
                        UserId = userRef.UserId,
                        EmployeeId = employeeEvent.EmployeeId,
                        FirstName = employeeEvent.FirstName,
                        LastName = employeeEvent.LastName,
                        Email = employeeEvent.Email,
                        PersonalEmail = employeeEvent.PersonalEmail,
                        DocumentNumber = employeeEvent.DocumentNumber,
                        RoleCode = userRef.RoleCode,
                        RoleName = userRef.RoleName,
                        EmployeeStores = employeeEvent.EmployeeStores
                            .Select(es => new DTOs.Requests.UserReference.EmployeeStoreReferenceRequestDto
                            {
                                StoreId = es.StoreId,
                                AssignmentDate = es.AssignmentDate
                            })
                            .ToArray(),
                    });
                }

                _logger.LogInformation("[SUBSCRIPTION-TRACE] EmployeeUpdated processed successfully - EmployeeId: {EmployeeId}", employeeEvent.EmployeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SUBSCRIPTION-ERROR] Error processing EmployeeUpdated - EmployeeId: {EmployeeId}", employeeEvent.EmployeeId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task HandleEmployeeDeletedAsync(EmployeeDeletedEvent employeeEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SUBSCRIPTION-TRACE] EmployeeDeleted event received - EmployeeId: {EmployeeId}, EventId: {EventId}", 
                employeeEvent.EmployeeId, employeeEvent.EventId);

            try
            {
                await _userReferenceService.Delete(employeeEvent.EmployeeId);
                _logger.LogInformation("[SUBSCRIPTION-TRACE] EmployeeDeleted processed successfully - EmployeeId: {EmployeeId}", employeeEvent.EmployeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SUBSCRIPTION-ERROR] Error processing EmployeeDeleted - EmployeeId: {EmployeeId}", employeeEvent.EmployeeId);
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
                            await HandleEmployeeUpdatedAsync(updatedEvent.Data, cancellationToken);
                        break;

                    case var key when key.Contains("employee.events.deleted"):
                        var deletedEvent = JsonConvert.DeserializeObject<EventWrapper<EmployeeDeletedEvent>>(message);
                        if (deletedEvent != null)
                            await HandleEmployeeDeletedAsync(deletedEvent.Data, cancellationToken);
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
    }
}