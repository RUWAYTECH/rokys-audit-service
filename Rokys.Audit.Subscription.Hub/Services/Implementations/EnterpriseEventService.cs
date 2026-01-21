using Microsoft.Extensions.Logging;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Subscription.Hub.Services.Interfaces;
using Ruway.Events.Command.Interfaces.Events;

namespace Rokys.Audit.Subscription.Hub.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de manejo de eventos de la empresa
    /// </summary>
    public class EnterpriseEventService : IEnterpriseEventService
    {
        private readonly ILogger<EnterpriseEventService> _logger;
        private readonly IEnterpriseService _enterpriseService;

        public EnterpriseEventService(
            ILogger<EnterpriseEventService> logger,
            IEnterpriseService enterpriseService)
        {
            _logger = logger;
            _enterpriseService = enterpriseService;
        }

        public async Task HandleEnterpriseCreatedAsync(EnterpriseCreatedEvent StoreEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseCreated event received at {Timestamp}. Code: {Code}, Name: {Name}, EnterpriseId: {EnterpriseId}",
                DateTime.UtcNow,
                StoreEvent.Code,
                StoreEvent.Name,
                StoreEvent.EnterpriseId);

            try
            {
                await _enterpriseService.Create(new DTOs.Requests.Enterprise.EnterpriseRequestDto
                {
                    Name = StoreEvent.Name,
                    Code = StoreEvent.Code,
                    Address = StoreEvent.Address,
                    PrimaryColor = StoreEvent.PrimaryColor,
                    SecondaryColor = StoreEvent.SecondaryColor,
                    AccentColor = StoreEvent.AccentColor,
                    BackgroundColor = StoreEvent.BackgroundColor,
                    TextColor = StoreEvent.TextColor,
                    LogoData = StoreEvent.LogoData,
                    LogoContentType = StoreEvent.LogoContentType,
                    LogoFileName = StoreEvent.LogoFileName
                });

                _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseCreated event processed successfully at {Timestamp}. EnterpriseId: {EnterpriseId}, Code: {Code}",
                    DateTime.UtcNow,
                    StoreEvent.EnterpriseId,
                    StoreEvent.Code);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseCreated event processed successfully at {Timestamp}. EnterpriseId: {EnterpriseId}, Code: {Code}, Error: {ErrorMessage}",
                    DateTime.UtcNow,
                    StoreEvent.EnterpriseId,
                    StoreEvent.Code,
                    ex.Message);
                throw;
            }
        }

        public async Task HandleEnterpriseDeletedAsync(EnterpriseDeletedEvent StoreEvent, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task HandleEnterpriseUpdatedAsync(EnterpriseUpdatedEvent StoreEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseUpdated event received at {Timestamp}. Code: {Code}, Name: {Name}, EnterpriseId: {EnterpriseId}",
                DateTime.UtcNow,
                StoreEvent.Code,
                StoreEvent.Name,
                StoreEvent.EnterpriseId);

            try
            {
                await _enterpriseService.Update(StoreEvent.EnterpriseId, new DTOs.Requests.Enterprise.EnterpriseUpdateRequestDto
                {
                    EnterpriseId = StoreEvent.EnterpriseId,
                    Name = StoreEvent.Name,
                    Code = StoreEvent.Code,
                    Address = StoreEvent.Address,
                    PrimaryColor = StoreEvent.PrimaryColor,
                    SecondaryColor = StoreEvent.SecondaryColor,
                    AccentColor = StoreEvent.AccentColor,
                    BackgroundColor = StoreEvent.BackgroundColor,
                    TextColor = StoreEvent.TextColor,
                    LogoData = StoreEvent.LogoData,
                    LogoContentType = StoreEvent.LogoContentType,
                    LogoFileName = StoreEvent.LogoFileName
                });
                _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseUpdated event processed successfully at {Timestamp}. EnterpriseId: {EnterpriseId}, Code: {Code}",
                   DateTime.UtcNow,
                   StoreEvent.EnterpriseId,
                   StoreEvent.Code);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseUpdated event processed successfully at {Timestamp}. EnterpriseId: {EnterpriseId}, Code: {Code}, Error: {ErrorMessage}",
                   DateTime.UtcNow,
                   StoreEvent.EnterpriseId,
                   StoreEvent.Code,
                   ex.Message);
                throw;
            }
        }
    }
}
