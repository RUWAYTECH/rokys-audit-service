using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Subscription.Hub.Services.Interfaces;
using Ruway.Events.Command.Interfaces.Events;

namespace Rokys.Audit.Subscription.Hub.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de manejo de eventos de empleados
    /// </summary>
    public class StoreEventService : IStoreEventService
    {
        private readonly ILogger<StoreEventService> _logger;
        private readonly IStoreService _storeService;

        public StoreEventService(
            ILogger<StoreEventService> logger,
            IStoreService StoreService)
        {
            _logger = logger;
            _storeService = StoreService;
        }

        /// <inheritdoc />
        public async Task HandleStoreCreatedAsync(StoreCreatedEvent StoreEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                await _storeService.Create(new DTOs.Requests.Store.StoreRequestDto
                {
                    Code = StoreEvent.Code ?? "",
                    Name = StoreEvent.Name,
                    Address = StoreEvent.Address,
                    EnterpriseId = StoreEvent.EnterpriseId,
                    Email = StoreEvent.Email
                });



                _logger.LogInformation("Successfully processed Store created event for Store ID: {StoreId}",
                    StoreEvent.StoreId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Store created event for Store ID: {StoreId}",
                    StoreEvent.StoreId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task HandleStoreUpdatedAsync(StoreUpdatedEvent StoreEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                var Store = await _storeService.GetById(StoreEvent.StoreId);
                if (Store.Data != null)
                {
                    var userRef = Store.Data;
                    _logger.LogWarning("User reference not found for Store ID: {StoreId}. Creating new reference.",
                        StoreEvent.StoreId);
                    await _storeService.Update(userRef.StoreId, new DTOs.Requests.Store.StoreRequestDto
                    {
                        Code = StoreEvent.Code ?? "",
                        Name = StoreEvent.Name,
                        Address = StoreEvent.Address,
                        EnterpriseId = StoreEvent.EnterpriseId,
                        Email = StoreEvent.Email
                    });
                    return;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Store updated event for Store ID: {StoreId}",
                    StoreEvent.StoreId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task HandleStoreDeletedAsync(StoreDeletedEvent StoreEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                await _storeService.Delete(StoreEvent.StoreId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Store deleted event for Store ID: {StoreId}",
                    StoreEvent.StoreId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task HandleGenericStoreEventAsync(string message, string routingKey, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Processing generic Store event with routing key: {RoutingKey}", routingKey);

                // Intentar deserializar como diferentes tipos de eventos
                await TryProcessAsTypedEvent(message, routingKey, cancellationToken);

                _logger.LogDebug("Successfully processed generic Store event with routing key: {RoutingKey}", routingKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing generic Store event with routing key: {RoutingKey}. Message: {Message}",
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
                    case var key when key.Contains("Store.events.created"):
                        var createdEvent = JsonConvert.DeserializeObject<EventWrapper<StoreCreatedEvent>>(message);
                        if (createdEvent != null)
                            await HandleStoreCreatedAsync(createdEvent.Data, cancellationToken);
                        break;

                    case var key when key.Contains("Store.events.updated"):
                        var updatedEvent = JsonConvert.DeserializeObject<EventWrapper<StoreUpdatedEvent>>(message);
                        if (updatedEvent != null)
                            await HandleStoreUpdatedAsync(updatedEvent.Data, cancellationToken);
                        break;

                    case var key when key.Contains("Store.events.deleted"):
                        var deletedEvent = JsonConvert.DeserializeObject<EventWrapper<StoreDeletedEvent>>(message);
                        if (deletedEvent != null)
                            await HandleStoreDeletedAsync(deletedEvent.Data, cancellationToken);
                        break;

                    default:
                        _logger.LogWarning("Unknown Store event routing key: {RoutingKey}", routingKey);
                        break;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize Store event message: {Message}", message);
            }
        }
    }
}