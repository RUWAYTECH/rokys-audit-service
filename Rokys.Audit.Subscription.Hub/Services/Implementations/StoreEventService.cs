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
                    Code = StoreEvent.Code,
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
                _logger.LogInformation("Successfully processed Store update event for Store ID: {StoreId}",
                   StoreEvent.StoreId);

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
    }
}