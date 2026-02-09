using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.Services.Services
{
    public class KPIReportsService : IKPIReportsService
    {
        private readonly IPeriodAuditRepository _periodAuditRepository;
        private readonly ILogger<KPIReportsService> _logger;
        private readonly IAuditStatusRepository _auditStatusRepository;
        private readonly IAMapper _mapper;
        private readonly IScaleCompanyRepository _scaleCompanyRepository;
        private readonly IPeriodAuditParticipantRepository _periodAuditParticipantRepository;
        private readonly IPeriodAuditTableScaleTemplateResultRepository _periodAuditTableScaleTemplateResultRepository;

        public KPIReportsService(
            IPeriodAuditRepository periodAuditRepository,
            ILogger<KPIReportsService> logger,
            IAuditStatusRepository auditStatusRepository,
            IAMapper mapper,
            IScaleCompanyRepository scaleCompanyRepository,
            IPeriodAuditParticipantRepository periodAuditParticipantRepository,
            IPeriodAuditTableScaleTemplateResultRepository periodAuditTableScaleTemplateResultRepository)
        {
            _periodAuditRepository = periodAuditRepository;
            _logger = logger;
            _auditStatusRepository = auditStatusRepository;
            _mapper = mapper;
            _scaleCompanyRepository = scaleCompanyRepository;
            _periodAuditParticipantRepository = periodAuditParticipantRepository;
            _periodAuditTableScaleTemplateResultRepository = periodAuditTableScaleTemplateResultRepository;
        }

        public async Task<ResponseDto<object>> GetGeneralKPIsAsync(int year, Guid[] enterpriseIds, Guid? enterpriseGroupingId)
        {
            var response = ResponseDto.Create<object>();
            try
            {
                _logger.LogInformation("Obteniendo KPIs generales para el año {Year}", year);

                // Obtener auditorías del año especificado
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31, 23, 59, 59);

                Expression<Func<PeriodAudit, bool>> baseFilter = x => x.CreationDate >= startDate &&
                    x.CreationDate <= endDate && x.IsActive
                    && x.AuditStatus != null && x.AuditStatus.Code == AuditStatusCode.Completed;

                if (enterpriseIds != null && enterpriseIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => enterpriseIds.Contains(x.Store.EnterpriseId));
                }

                if (enterpriseGroupingId != null && enterpriseGroupingId.HasValue)
                {
                    baseFilter = baseFilter.AndAlso(x => x.Store.Enterprise.EnterpriseGroups.Any(eg => eg.EnterpriseGroupingId == enterpriseGroupingId && eg.IsActive));
                }

                var periodAudits = await _periodAuditRepository.GetAsync(
                    filter: baseFilter,
                    includeProperties:
                    [
                        x => x.Store.Enterprise,
                        x => x.AuditStatus
                    ]);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener KPIs generales");
                
                response.Messages.Add( new ApplicationMessage { Key = "Error", Message = ex.Message } );
            }

            return response;
        }
    }
}
