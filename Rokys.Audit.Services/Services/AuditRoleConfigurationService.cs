using Microsoft.Extensions.Logging;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.AuditRoleConfiguration;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.Services.Services
{
    public class AuditRoleConfigurationService : IAuditRoleConfigurationService
    {
        private readonly IAuditRoleConfigurationRepository _auditRoleConfigurationRepository;
        private readonly IAMapper _mapper;
        private readonly ILogger<AuditRoleConfigurationService> _logger;

        public AuditRoleConfigurationService(
            IAuditRoleConfigurationRepository auditRoleConfigurationRepository,
            IAMapper mapper,
            ILogger<AuditRoleConfigurationService> logger)
        {
            _auditRoleConfigurationRepository = auditRoleConfigurationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseDto<List<AuditRoleConfigurationResponseDto>>> GetActiveConfigurationsAsync()
        {
            var response = ResponseDto.Create<List<AuditRoleConfigurationResponseDto>>();
            
            try
            {
                var configurations = await _auditRoleConfigurationRepository.GetActiveConfigurationsOrderedAsync();
                response.Data = _mapper.Map<List<AuditRoleConfigurationResponseDto>>(configurations);
                
                _logger.LogInformation("Retrieved {Count} active audit role configurations", configurations.Count());
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active audit role configurations");
                response.Messages.Add(new ApplicationMessage
                {
                    Message = "Error al obtener las configuraciones de roles de auditoría",
                    MessageType = ApplicationMessageType.Error
                });
                return response;
            }
        }
    }
}