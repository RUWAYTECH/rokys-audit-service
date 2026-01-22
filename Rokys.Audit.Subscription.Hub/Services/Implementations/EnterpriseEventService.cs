using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory _scopeFactory;

        public EnterpriseEventService(
            ILogger<EnterpriseEventService> logger,
            IEnterpriseService enterpriseService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _enterpriseService = enterpriseService;
            _scopeFactory = serviceScopeFactory;
        }

        public async Task HandleEnterpriseCreatedAsync(EnterpriseCreatedEvent EnterpriseEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseCreated event received at {Timestamp}. Code: {Code}, Name: {Name}, EnterpriseId: {EnterpriseId}",
                DateTime.UtcNow,
                EnterpriseEvent.Code,
                EnterpriseEvent.Name,
                EnterpriseEvent.EnterpriseId);

            try
            {
                await _enterpriseService.Create(new DTOs.Requests.Enterprise.EnterpriseCreateRequestDto
                {
                    EnterpriseId = EnterpriseEvent.EnterpriseId,
                    Name = EnterpriseEvent.Name,
                    Code = EnterpriseEvent.Code,
                    Address = EnterpriseEvent.Address,
                    PrimaryColor = EnterpriseEvent.PrimaryColor,
                    SecondaryColor = EnterpriseEvent.SecondaryColor,
                    AccentColor = EnterpriseEvent.AccentColor,
                    BackgroundColor = EnterpriseEvent.BackgroundColor,
                    TextColor = EnterpriseEvent.TextColor,
                    LogoData = EnterpriseEvent.LogoData,
                    LogoContentType = EnterpriseEvent.LogoContentType,
                    LogoFileName = EnterpriseEvent.LogoFileName
                });

                _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseCreated event processed successfully at {Timestamp}. EnterpriseId: {EnterpriseId}, Code: {Code}",
                    DateTime.UtcNow,
                    EnterpriseEvent.EnterpriseId,
                    EnterpriseEvent.Code);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseCreated event processed successfully at {Timestamp}. EnterpriseId: {EnterpriseId}, Code: {Code}, Error: {ErrorMessage}",
                    DateTime.UtcNow,
                    EnterpriseEvent.EnterpriseId,
                    EnterpriseEvent.Code,
                    ex.Message);
                throw;
            }
        }

        public async Task HandleEnterpriseDeletedAsync(EnterpriseDeletedEvent EnterpriseEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseDeleted event received at {Timestamp}. EnterpriseId: {EnterpriseId}",
                DateTime.UtcNow,
                EnterpriseEvent.EnterpriseId);
            try
            {
                await _enterpriseService.Delete(EnterpriseEvent.EnterpriseId);
                _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseDeleted event processed successfully at {Timestamp}. EnterpriseId: {EnterpriseId}",
                   DateTime.UtcNow,
                   EnterpriseEvent.EnterpriseId);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseDeleted event processed successfully at {Timestamp}. EnterpriseId: {EnterpriseId}, Error: {ErrorMessage}",
                   DateTime.UtcNow,
                   EnterpriseEvent.EnterpriseId,
                   ex.Message);
                throw;
            }
        }

        public async Task HandleEnterpriseUpdatedAsync(EnterpriseUpdatedEvent EnterpriseEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseUpdated event received at {Timestamp}. Code: {Code}, Name: {Name}, EnterpriseId: {EnterpriseId}",
                DateTime.UtcNow,
                EnterpriseEvent.Code,
                EnterpriseEvent.Name,
                EnterpriseEvent.EnterpriseId);

            try
            {
                await _enterpriseService.Update(EnterpriseEvent.EnterpriseId, new DTOs.Requests.Enterprise.EnterpriseUpdateRequestDto
                {
                    Name = EnterpriseEvent.Name,
                    Code = EnterpriseEvent.Code,
                    Address = EnterpriseEvent.Address,
                    PrimaryColor = EnterpriseEvent.PrimaryColor,
                    SecondaryColor = EnterpriseEvent.SecondaryColor,
                    AccentColor = EnterpriseEvent.AccentColor,
                    BackgroundColor = EnterpriseEvent.BackgroundColor,
                    TextColor = EnterpriseEvent.TextColor,
                    LogoData = EnterpriseEvent.LogoData,
                    LogoContentType = EnterpriseEvent.LogoContentType,
                    LogoFileName = EnterpriseEvent.LogoFileName
                });
                   
                _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseUpdated event processed successfully at {Timestamp}. EnterpriseId: {EnterpriseId}, Code: {Code}",
                   DateTime.UtcNow,
                   EnterpriseEvent.EnterpriseId,
                   EnterpriseEvent.Code);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("[SUBSCRIPTION-TRACE] EnterpriseUpdated event processed successfully at {Timestamp}. EnterpriseId: {EnterpriseId}, Code: {Code}, Error: {ErrorMessage}",
                   DateTime.UtcNow,
                   EnterpriseEvent.EnterpriseId,
                   EnterpriseEvent.Code,
                   ex.Message);
                throw;
            }
        }
    }
}
