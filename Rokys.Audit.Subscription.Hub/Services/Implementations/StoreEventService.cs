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
            _logger.LogInformation("[SUBSCRIPTION-TRACE] StoreCreated event received at {Timestamp}. StoreId: {StoreId}, Code: {Code}, Name: {Name}, EnterpriseId: {EnterpriseId}",
                DateTime.UtcNow,
                StoreEvent.StoreId,
                StoreEvent.Code,
                StoreEvent.Name,
                StoreEvent.EnterpriseId);

            try
            {
                await _storeService.Create(new DTOs.Requests.Store.StoreRequestDto
                {
                    Code = StoreEvent.Code,
                    Name = StoreEvent.Name,
                    Address = StoreEvent.Address,
                    EnterpriseId = StoreEvent.EnterpriseId,
                    Email = StoreEvent.Email
                });

                _logger.LogInformation("[SUBSCRIPTION-TRACE] StoreCreated event processed successfully at {Timestamp}. StoreId: {StoreId}, Code: {Code}",
                    DateTime.UtcNow,
                    StoreEvent.StoreId,
                    StoreEvent.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SUBSCRIPTION-ERROR] Error processing StoreCreated event at {Timestamp}. StoreId: {StoreId}, Code: {Code}, Error: {ErrorMessage}",
                    DateTime.UtcNow,
                    StoreEvent.StoreId,
                    StoreEvent.Code,
                    ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task HandleStoreUpdatedAsync(StoreUpdatedEvent StoreEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SUBSCRIPTION-TRACE] StoreUpdated event received at {Timestamp}. StoreId: {StoreId}, Code: {Code}, Name: {Name}, EnterpriseId: {EnterpriseId}",
                DateTime.UtcNow,
                StoreEvent.StoreId,
                StoreEvent.Code,
                StoreEvent.Name,
                StoreEvent.EnterpriseId);

            try
            {
                if (StoreEvent != null)
                {
                    await _storeService.Update(StoreEvent.StoreId, new DTOs.Requests.Store.StoreRequestDto
                    {
                        Code = StoreEvent.Code ?? "",
                        Name = StoreEvent.Name,
                        Address = StoreEvent.Address,
                        EnterpriseId = StoreEvent.EnterpriseId,
                        Email = StoreEvent.Email
                    });
                }

                _logger.LogInformation("[SUBSCRIPTION-TRACE] StoreUpdated event processed successfully at {Timestamp}. StoreId: {StoreId}, Code: {Code}",
                    DateTime.UtcNow,
                    StoreEvent.StoreId,
                    StoreEvent.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SUBSCRIPTION-ERROR] Error processing StoreUpdated event at {Timestamp}. StoreId: {StoreId}, Code: {Code}, Error: {ErrorMessage}",
                    DateTime.UtcNow,
                    StoreEvent.StoreId,
                    StoreEvent.Code,
                    ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task HandleStoreDeletedAsync(StoreDeletedEvent StoreEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SUBSCRIPTION-TRACE] StoreDeleted event received at {Timestamp}. StoreId: {StoreId}",
                DateTime.UtcNow,
                StoreEvent.StoreId);

            try
            {
                await _storeService.Delete(StoreEvent.StoreId);

                _logger.LogInformation("[SUBSCRIPTION-TRACE] StoreDeleted event processed successfully at {Timestamp}. StoreId: {StoreId}",
                    DateTime.UtcNow,
                    StoreEvent.StoreId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SUBSCRIPTION-ERROR] Error processing StoreDeleted event at {Timestamp}. StoreId: {StoreId}, Error: {ErrorMessage}",
                    DateTime.UtcNow,
                    StoreEvent.StoreId,
                    ex.Message);
                throw;
            }
        }
    }
}