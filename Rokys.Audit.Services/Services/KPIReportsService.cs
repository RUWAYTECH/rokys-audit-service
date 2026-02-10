using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.KpiReports;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.KpiReports;
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
        private readonly IPeriodAuditGroupResultRepository _periodAuditGroupResultRepository;

        public KPIReportsService(
            IPeriodAuditRepository periodAuditRepository,
            ILogger<KPIReportsService> logger,
            IAuditStatusRepository auditStatusRepository,
            IAMapper mapper,
            IScaleCompanyRepository scaleCompanyRepository,
            IPeriodAuditParticipantRepository periodAuditParticipantRepository,
            IPeriodAuditTableScaleTemplateResultRepository periodAuditTableScaleTemplateResultRepository,
            IPeriodAuditGroupResultRepository periodAuditGroupResultRepository)
        {
            _periodAuditRepository = periodAuditRepository;
            _logger = logger;
            _auditStatusRepository = auditStatusRepository;
            _mapper = mapper;
            _scaleCompanyRepository = scaleCompanyRepository;
            _periodAuditParticipantRepository = periodAuditParticipantRepository;
            _periodAuditTableScaleTemplateResultRepository = periodAuditTableScaleTemplateResultRepository;
            _periodAuditGroupResultRepository = periodAuditGroupResultRepository;
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

        public async Task<ResponseDto<List<DataByParticipantResponseDto>>> GetDataByParticipantAsync(DataByParticipantRequestDto request)
        {
            var response = ResponseDto.Create<List<DataByParticipantResponseDto>>();
            try
            {
                _logger.LogInformation("Obteniendo datos por participante");

                // Construir filtro base
                Expression<Func<PeriodAudit, bool>> baseFilter = x => x.IsActive
                    && x.AuditStatus != null && x.AuditStatus.Code == AuditStatusCode.Completed && x.Store.Enterprise.EnterpriseGroups.Any(eg => eg.EnterpriseGroupingId == request.EnterpriseGroupingId && eg.IsActive);

                // Filtrar por EnterpriseIds si se proporciona
                if (request.EnterpriseIds != null && request.EnterpriseIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => request.EnterpriseIds.Contains(x.Store.EnterpriseId));
                }

                // Filtrar por StoreIds si se proporciona
                if (request.StoreIds != null && request.StoreIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.StoreId.HasValue && request.StoreIds.Contains(x.StoreId.Value));
                }

                // Filtrar por meses si se proporcionan
                if (!string.IsNullOrEmpty(request.Months))
                {
                    var monthsList = request.Months.Split(',').Select(m => int.Parse(m.Trim())).ToList();
                    baseFilter = baseFilter.AndAlso(x => monthsList.Contains(x.StartDate.Month));
                }

                // Filtrar por participantes (en BD)
                if (request.UserReferenceIds != null && request.UserReferenceIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap => 
                        pap.IsActive 
                        && pap.RoleCodeSnapshot == request.ParticipantType
                        && request.UserReferenceIds.Contains(pap.UserReferenceId)));
                }
                else
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap => 
                        pap.IsActive 
                        && pap.RoleCodeSnapshot == request.ParticipantType));
                }

                // Obtener auditorías con participantes
                var periodAudits = await _periodAuditRepository.GetAsync(
                    filter: baseFilter,
                    includeProperties:
                    [
                        x => x.Store,
                        x => x.PeriodAuditParticipants,
                        x => x.AuditStatus
                    ]);

                // Extraer datos de participantes
                var participantsData = periodAudits
                    .SelectMany(pa => pa.PeriodAuditParticipants
                        .Where(pap => pap.IsActive 
                            && pap.RoleCodeSnapshot == request.ParticipantType
                            && (request.UserReferenceIds == null 
                                || request.UserReferenceIds.Length == 0 
                                || request.UserReferenceIds.Contains(pap.UserReferenceId)))
                        .Select(pap => new
                        {
                            pa.StoreId,
                            StoreName = pa.Store?.Name,
                            pa.StartDate.Month,
                            pap.UserReferenceId,
                            Score = pa.ScoreValue
                        }))
                    .ToList();

                // Agrupar y calcular promedios
                var groupedData = participantsData
                    .GroupBy(x => new
                    {
                        x.StoreId,
                        x.StoreName,
                        x.Month,
                        x.UserReferenceId
                    })
                    .Select(g => new
                    {
                        g.Key.StoreId,
                        g.Key.StoreName,
                        Month = g.Key.Month.ToString("00"),
                        g.Key.UserReferenceId,
                        Average = Math.Round(g.Average(x => x.Score), 2)
                    })
                    .ToList();

                // Obtener información de usuarios
                var userReferenceIds = groupedData.Select(x => x.UserReferenceId).Distinct().ToList();
                var userReferences = await _periodAuditParticipantRepository.GetAsync(
                    filter: x => userReferenceIds.Contains(x.UserReferenceId),
                    includeProperties: [x => x.UserReference]);

                var userReferenceDict = userReferences
                    .Where(x => x.UserReference != null)
                    .GroupBy(x => x.UserReferenceId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().UserReference
                    );

                // Generar resultado final
                var result = groupedData.Select(x => new DataByParticipantResponseDto
                {
                    StoreId = x.StoreId?.ToString() ?? "",
                    Store = x.StoreName ?? "",
                    Month = x.Month,
                    UserReferenceId = x.UserReferenceId.ToString(),
                    UserName = userReferenceDict.TryGetValue(x.UserReferenceId, out var userRef)
                        ? $"{userRef.FirstName} {userRef.LastName}".Trim()
                        : "",
                    Average = x.Average
                }).ToList();

                response.Data = result;
                _logger.LogInformation("Se generaron {Count} registros en el reporte por participante", result.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos por participante");
                response.Messages.Add(new ApplicationMessage { Key = "Error", Message = ex.Message });
            }

            return response;
        }

        public async Task<ResponseDto<List<StoreRankingResponseDto>>> GetTopStoresRankingAsync(TopRankingRequestDto request)
        {
            var response = ResponseDto.Create<List<StoreRankingResponseDto>>();
            try
            {
                var isBestRanking = request.RankingType.Equals("best", StringComparison.CurrentCultureIgnoreCase);

                var rankingTypeDisplay = isBestRanking ? "mejores tiendas" : "tiendas con mayor riesgo";
                _logger.LogInformation("Obteniendo Top {TopCount} {RankingType}", request.TopCount, rankingTypeDisplay);

                // Validar TopCount
                if (request.TopCount <= 0)
                {
                    response.Messages.Add(new ApplicationMessage { Key = "ValidationError", Message = "TopCount debe ser mayor a 0" });
                    return response;
                }

                // Construir filtro base
                Expression<Func<PeriodAudit, bool>> baseFilter = x => x.IsActive
                    && x.AuditStatus != null && x.AuditStatus.Code == AuditStatusCode.Completed
                    && x.Store.Enterprise.EnterpriseGroups.Any(eg => eg.EnterpriseGroupingId == request.EnterpriseGroupingId && eg.IsActive);

                // Filtrar por EnterpriseIds si se proporciona
                if (request.EnterpriseIds != null && request.EnterpriseIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => request.EnterpriseIds.Contains(x.Store.EnterpriseId));
                }

                // Filtrar por meses si se proporciona
                if (!string.IsNullOrEmpty(request.Months))
                {
                    var monthsList = request.Months.Split(',').Select(m => int.Parse(m.Trim())).ToList();
                    baseFilter = baseFilter.AndAlso(x => monthsList.Contains(x.StartDate.Month));
                }

                // Filtrar por SupervisorIds si se proporciona
                if (request.SupervisorIds != null && request.SupervisorIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code
                        && request.SupervisorIds.Contains(pap.UserReferenceId)));
                }

                // Filtrar por AuditorIds si se proporciona
                if (request.AuditorIds != null && request.AuditorIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.Auditor.Code
                        && request.AuditorIds.Contains(pap.UserReferenceId)));
                }

                // Obtener auditorías
                var periodAudits = await _periodAuditRepository.GetAsync(
                    filter: baseFilter,
                    includeProperties: [x => x.Store]);

                // Agrupar por tienda y calcular promedio
                var storeAveragesQuery = periodAudits
                    .GroupBy(pa => new
                    {
                        pa.StoreId,
                        StoreName = pa.Store?.Name,
                        StoreCode = pa.Store?.Code
                    })
                    .Select(g => new
                    {
                        g.Key.StoreId,
                        g.Key.StoreName,
                        g.Key.StoreCode,
                        Average = Math.Round(g.Average(x => x.ScoreValue), 2),
                        AuditCount = g.Count()
                    });

                // Ordenar según el tipo de ranking
                var storeAverages = isBestRanking
                    ? storeAveragesQuery.OrderByDescending(x => x.Average).Take(request.TopCount).ToList()
                    : storeAveragesQuery.OrderBy(x => x.Average).Take(request.TopCount).ToList();

                // Generar resultado
                var result = storeAverages.Select((x, index) => new StoreRankingResponseDto
                {
                    Ranking = index + 1,
                    StoreId = x.StoreId?.ToString() ?? "",
                    Store = x.StoreName ?? "",
                    StoreCode = x.StoreCode ?? "",
                    Average = x.Average,
                    AuditCount = x.AuditCount
                }).ToList();

                response.Data = result;
                _logger.LogInformation("Se generaron {Count} registros en el TOP {TopCount} {RankingType}", result.Count, request.TopCount, rankingTypeDisplay);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ranking de tiendas");
                response.Messages.Add(new ApplicationMessage { Key = "Error", Message = ex.Message });
            }

            return response;
        }

        public async Task<ResponseDto<List<DataByAuditableGroupResponseDto>>> GetDataByAuditableGroupAsync(DataByAuditableGroupRequestDto request)
        {
            var response = ResponseDto.Create<List<DataByAuditableGroupResponseDto>>();
            try
            {
                _logger.LogInformation("Obteniendo reporte de grupos auditables");

                // Construir filtro base para auditorías
                Expression<Func<PeriodAudit, bool>> baseFilter = x => x.IsActive
                    && x.AuditStatus != null && x.AuditStatus.Code == AuditStatusCode.Completed
                    && x.Store.Enterprise.EnterpriseGroups.Any(eg => eg.EnterpriseGroupingId == request.EnterpriseGroupingId && eg.IsActive);

                // Filtrar por EnterpriseIds si se proporciona
                if (request.EnterpriseIds != null && request.EnterpriseIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => request.EnterpriseIds.Contains(x.Store.EnterpriseId));
                }

                // Filtrar por StoreIds si se proporciona
                if (request.StoreIds != null && request.StoreIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.StoreId.HasValue && request.StoreIds.Contains(x.StoreId.Value));
                }

                // Filtrar por meses si se proporciona
                if (!string.IsNullOrEmpty(request.Months))
                {
                    var monthsList = request.Months.Split(',').Select(m => int.Parse(m.Trim())).ToList();
                    baseFilter = baseFilter.AndAlso(x => monthsList.Contains(x.StartDate.Month));
                }

                // Filtrar por SupervisorIds si se proporciona
                if (request.SupervisorIds != null && request.SupervisorIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code
                        && request.SupervisorIds.Contains(pap.UserReferenceId)));
                }

                // Obtener auditorías
                var periodAudits = await _periodAuditRepository.GetAsync(
                    filter: baseFilter,
                    includeProperties: [x => x.Store]);

                var periodAuditIds = periodAudits.Select(x => x.PeriodAuditId).ToList();

                if (!periodAuditIds.Any())
                {
                    response.Data = new List<DataByAuditableGroupResponseDto>();
                    return response;
                }

                // Obtener resultados por grupo auditable (Group)
                Expression<Func<PeriodAuditGroupResult, bool>> groupResultFilter = x => x.IsActive
                    && periodAuditIds.Contains(x.PeriodAuditId);

                if (request.GroupIds != null && request.GroupIds.Length > 0)
                {
                    groupResultFilter = groupResultFilter.AndAlso(x => request.GroupIds.Contains(x.GroupId));
                }

                var groupResults = await _periodAuditGroupResultRepository.GetAsync(
                    filter: groupResultFilter,
                    includeProperties: [x => x.Group]);

                var result = groupResults
                    .GroupBy(x => new
                    {
                        GroupId = x.GroupId,
                        GroupCode = x.Group?.Code,
                        GroupName = x.Group?.Name
                    })
                    .Select(g => new DataByAuditableGroupResponseDto
                    {
                        GroupId = g.Key.GroupId.ToString(),
                        GroupCode = g.Key.GroupCode ?? "",
                        GroupName = g.Key.GroupName ?? "",
                        Average = Math.Round(g.Average(x => x.ScoreValue), 2),
                        AuditCount = g.Count()
                    })
                    .OrderByDescending(x => x.Average)
                    .ToList();

                response.Data = result;
                _logger.LogInformation("Reporte generado: {GroupCount} grupos auditables", result.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de grupos auditables");
                response.Messages.Add(new ApplicationMessage { Key = "Error", Message = ex.Message });
            }

            return response;
        }

        public async Task<ResponseDto<List<DataBySupervisorStoreResponseDto>>> GetDataBySupervisorStoreAsync(TopRankingRequestDto request)
        {
            var response = ResponseDto.Create<List<DataBySupervisorStoreResponseDto>>();
            try
            {
                _logger.LogInformation("Obteniendo datos por supervisor y tienda");

                // Construir filtro base para auditorías
                Expression<Func<PeriodAudit, bool>> baseFilter = x => x.IsActive
                    && x.AuditStatus != null && x.AuditStatus.Code == AuditStatusCode.Completed
                    && x.Store.Enterprise.EnterpriseGroups.Any(eg => eg.EnterpriseGroupingId == request.EnterpriseGroupingId && eg.IsActive)
                    && x.PeriodAuditParticipants.Any(pap => pap.IsActive && pap.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code);

                // Filtrar por EnterpriseIds si se proporciona
                if (request.EnterpriseIds != null && request.EnterpriseIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => request.EnterpriseIds.Contains(x.Store.EnterpriseId));
                }

                // Filtrar por meses si se proporciona
                if (!string.IsNullOrEmpty(request.Months))
                {
                    var monthsList = request.Months.Split(',').Select(m => int.Parse(m.Trim())).ToList();
                    baseFilter = baseFilter.AndAlso(x => monthsList.Contains(x.StartDate.Month));
                }

                // Filtrar por SupervisorIds si se proporciona
                if (request.SupervisorIds != null && request.SupervisorIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code
                        && request.SupervisorIds.Contains(pap.UserReferenceId)));
                }

                // Filtrar por AuditorIds si se proporciona
                if (request.AuditorIds != null && request.AuditorIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.Auditor.Code
                        && request.AuditorIds.Contains(pap.UserReferenceId)));
                }

                // Obtener auditorías con participantes
                var periodAudits = await _periodAuditRepository.GetAsync(
                    filter: baseFilter,
                    includeProperties:
                    [
                        x => x.Store,
                        x => x.PeriodAuditParticipants,
                        x => x.AuditStatus
                    ]);

                // Extraer datos por supervisor y tienda
                var supervisorStoreData = periodAudits
                    .SelectMany(pa => pa.PeriodAuditParticipants
                        .Where(pap => pap.IsActive && pap.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code)
                        .Select(pap => new
                        {
                            pa.StoreId,
                            StoreName = pa.Store?.Name,
                            StoreCode = pa.Store?.Code,
                            SupervisorId = pap.UserReferenceId,
                            Score = pa.ScoreValue
                        }))
                    .ToList();

                // Agrupar y calcular promedios por supervisor y tienda
                var groupedData = supervisorStoreData
                    .GroupBy(x => new
                    {
                        x.SupervisorId,
                        x.StoreId,
                        x.StoreName,
                        x.StoreCode
                    })
                    .Select(g => new
                    {
                        g.Key.SupervisorId,
                        g.Key.StoreId,
                        g.Key.StoreName,
                        g.Key.StoreCode,
                        Average = Math.Round(g.Average(x => x.Score), 2),
                        AuditCount = g.Count()
                    })
                    .ToList();

                // Obtener información de supervisores
                var supervisorIds = groupedData.Select(x => x.SupervisorId).Distinct().ToList();
                var supervisorReferences = await _periodAuditParticipantRepository.GetAsync(
                    filter: x => supervisorIds.Contains(x.UserReferenceId),
                    includeProperties: [x => x.UserReference]);

                var supervisorDict = supervisorReferences
                    .Where(x => x.UserReference != null)
                    .GroupBy(x => x.UserReferenceId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().UserReference
                    );

                // Generar resultado final
                var result = groupedData.Select(x => new DataBySupervisorStoreResponseDto
                {
                    SupervisorId = x.SupervisorId.ToString(),
                    SupervisorName = supervisorDict.TryGetValue(x.SupervisorId, out var supervisor)
                        ? $"{supervisor.FirstName} {supervisor.LastName}".Trim()
                        : "",
                    StoreId = x.StoreId?.ToString() ?? "",
                    Store = x.StoreName ?? "",
                    StoreCode = x.StoreCode ?? "",
                    Average = x.Average,
                    AuditCount = x.AuditCount
                })
                .OrderBy(x => x.SupervisorName)
                .ThenByDescending(x => x.Average)
                .ToList();

                response.Data = result;
                _logger.LogInformation("Se generaron {Count} registros en el reporte por supervisor y tienda", result.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos por supervisor y tienda");
                response.Messages.Add(new ApplicationMessage { Key = "Error", Message = ex.Message });
            }

            return response;
        }
  }
}
