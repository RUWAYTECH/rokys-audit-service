using Microsoft.Extensions.Logging;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.PeriodAuditActionPlan;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Common.Helpers;

namespace Rokys.Audit.Services.Services
{
    public class PeriodAuditActionPlanService : IPeriodAuditActionPlanService
    {
        private readonly IPeriodAuditRepository _periodAuditRepository;
        private readonly IEnterpriseGroupRepository _enterpriseGroupRepository;
        private readonly ISystemConfigurationRepository _systemConfigurationRepository;
        private readonly IAMapper _mapper;
        private readonly ILogger _logger;

        public PeriodAuditActionPlanService(
            IPeriodAuditRepository periodAuditRepository,
            IEnterpriseGroupRepository enterpriseGroupRepository,
            ISystemConfigurationRepository systemConfigurationRepository,
            IAMapper mapper,
            ILogger<PeriodAuditActionPlanService> logger)
        {
            _periodAuditRepository = periodAuditRepository;
            _enterpriseGroupRepository = enterpriseGroupRepository;
            _systemConfigurationRepository = systemConfigurationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseDto<EnterpriseConfigurationResponseDto>> GetEnterpriseConfigurationByPeriodAuditId(Guid periodAuditId)
        {
            var response = ResponseDto.Create<EnterpriseConfigurationResponseDto>();
            try
            {
                // Obtener el PeriodAudit con sus relaciones
                var periodAudit = await _periodAuditRepository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditId == periodAuditId && x.IsActive,
                    includeProperties: [x => x.Store!.Enterprise]
                );

                if (periodAudit == null)
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "No se encontró la auditoría solicitada",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                if (periodAudit.Store == null || periodAudit.Store.Enterprise == null)
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "No se encontró la empresa asociada a la auditoría",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                var enterprise = periodAudit.Store.Enterprise;

                // Paso 1: Verificar si la empresa pertenece a un EnterpriseGroup
                var enterpriseGroup = await _enterpriseGroupRepository.GetFirstOrDefaultAsync(
                    filter: x => x.EnterpriseId == enterprise.EnterpriseId && x.IsActive,
                    includeProperties: [x => x.EnterpriseGrouping]
                );

                bool hasConfiguration = false;
                decimal configValue = 0;

                // Paso 2: Si pertenece a un grupo, buscar configuraciones usando el Code del EnterpriseGrouping
                if (enterpriseGroup != null && enterpriseGroup.EnterpriseGrouping != null)
                {
                    var enterpriseGroupingCode = enterpriseGroup.EnterpriseGrouping.Code;

                    var systemConfigurations = await _systemConfigurationRepository.GetFirstOrDefaultAsync(
                        a => a.ConfigKey == enterpriseGroupingCode && a.IsActive
                        && a.ReferenceCode == enterpriseGroupingCode
                        && a.ReferenceType == SystemConfigRelation.EnterpriseGrouping.Code
                    );

                    if (systemConfigurations != null)
                    {
                        hasConfiguration = true;
                        //usando el utilitario
                        configValue = ConfigurationValueConverter.ConvertToNumeric<decimal>(
                            systemConfigurations.ConfigValue,
                            systemConfigurations.DataType,
                            0m,
                            decimal.TryParse,
                            int.TryParse
                        );
                    }
                }

                response.Data = new EnterpriseConfigurationResponseDto
                {
                    EnterpriseId = enterprise.EnterpriseId,
                    EnterpriseName = enterprise.Name,
                    EnterpriseCode = enterprise.Code,
                    HasConfiguration = hasConfiguration,
                    ConfigurationValue = configValue
                };
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<EnterpriseConfigurationResponseDto>(ex.Message);
                _logger.LogError(ex, "Error al obtener configuración de empresa para PeriodAuditId: {PeriodAuditId}", periodAuditId);
            }
            return response;
        }
    }
}
