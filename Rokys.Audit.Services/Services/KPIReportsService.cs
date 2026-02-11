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
        private readonly ISystemConfigurationRepository _systemConfigurationRepository;

        public KPIReportsService(
            IPeriodAuditRepository periodAuditRepository,
            ILogger<KPIReportsService> logger,
            IAuditStatusRepository auditStatusRepository,
            IAMapper mapper,
            IScaleCompanyRepository scaleCompanyRepository,
            IPeriodAuditParticipantRepository periodAuditParticipantRepository,
            IPeriodAuditTableScaleTemplateResultRepository periodAuditTableScaleTemplateResultRepository,
            IPeriodAuditGroupResultRepository periodAuditGroupResultRepository,
            ISystemConfigurationRepository systemConfigurationRepository)
        {
            _periodAuditRepository = periodAuditRepository;
            _logger = logger;
            _auditStatusRepository = auditStatusRepository;
            _mapper = mapper;
            _scaleCompanyRepository = scaleCompanyRepository;
            _periodAuditParticipantRepository = periodAuditParticipantRepository;
            _periodAuditTableScaleTemplateResultRepository = periodAuditTableScaleTemplateResultRepository;
            _periodAuditGroupResultRepository = periodAuditGroupResultRepository;
            _systemConfigurationRepository = systemConfigurationRepository;
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

                // Filtrar por fecha de inicio si se proporciona
                if (request.StartDate.HasValue)
                {
                    baseFilter = baseFilter.AndAlso(x => x.StartDate >= request.StartDate.Value);
                }

                // Filtrar por fecha de fin si se proporciona
                if (request.EndDate.HasValue)
                {
                    var endDate = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    baseFilter = baseFilter.AndAlso(x => x.StartDate <= endDate);
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

                // Filtrar por UnitManagerIds si se proporciona
                if (request.UnitManagerIds != null && request.UnitManagerIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.UnitManager.Code
                        && request.UnitManagerIds.Contains(pap.UserReferenceId)));
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

                // Extraer datos de participantes (solo del tipo especificado en ParticipantType)
                var participantsData = periodAudits
                    .SelectMany(pa => pa.PeriodAuditParticipants
                        .Where(pap => pap.IsActive && pap.RoleCodeSnapshot == request.ParticipantType)
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

                // Filtrar por fecha de inicio si se proporciona
                if (request.StartDate.HasValue)
                {
                    baseFilter = baseFilter.AndAlso(x => x.StartDate >= request.StartDate.Value);
                }

                // Filtrar por fecha de fin si se proporciona
                if (request.EndDate.HasValue)
                {
                    var endDate = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    baseFilter = baseFilter.AndAlso(x => x.StartDate <= endDate);
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

                // Filtrar por UnitManagerIds si se proporciona
                if (request.UnitManagerIds != null && request.UnitManagerIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.UnitManager.Code
                        && request.UnitManagerIds.Contains(pap.UserReferenceId)));
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

                // Obtener resultados por grupo auditable con sus puntos auditables
                Expression<Func<PeriodAuditGroupResult, bool>> groupResultFilter = x => x.IsActive
                    && periodAuditIds.Contains(x.PeriodAuditId);

                if (request.GroupIds != null && request.GroupIds.Length > 0)
                {
                    groupResultFilter = groupResultFilter.AndAlso(x => request.GroupIds.Contains(x.GroupId));
                }

                var groupResults = await _periodAuditGroupResultRepository.GetByPeriodAuditIdWithScaleResultsAsync(filter: groupResultFilter);

                // Agrupar por Group y calcular promedios
                var result = groupResults
                    .GroupBy(x => new
                    {
                        GroupCode = x.Group?.Code,
                        GroupName = x.Group?.Name
                    })
                    .Select(groupData => 
                    {
                        // Obtener todos los scale results de este grupo
                        var allScaleResults = groupData
                            .SelectMany(gr => gr.PeriodAuditScaleResults.Where(sr => sr.IsActive))
                            .ToList();

                        // Agrupar por ScaleGroup para obtener los puntos auditables
                        var auditablePoints = allScaleResults
                            .GroupBy(sr => new
                            {
                                ScaleCode = sr.ScaleGroup?.Code,
                                ScaleName = sr.ScaleGroup?.Name
                            })
                            .Select(scaleData => new AuditablePointsResponseDto
                            {
                                Code = scaleData.Key.ScaleCode ?? "",
                                Name = scaleData.Key.ScaleName ?? "",
                                Average = Math.Round(scaleData.Average(s => s.ScoreValue), 2),
                                AuditCount = scaleData.Count()
                            })
                            .OrderByDescending(ap => ap.Average)
                            .ToArray();

                        return new DataByAuditableGroupResponseDto
                        {
                            Code = groupData.Key.GroupCode ?? "",
                            Name = groupData.Key.GroupName ?? "",
                            Average = Math.Round(groupData.Average(x => x.ScoreValue), 2),
                            AuditCount = groupData.Count(),
                            AuditablePoints = auditablePoints
                        };
                    })
                    .OrderByDescending(x => x.Average)
                    .ToList();

                response.Data = result;
                _logger.LogInformation("Reporte generado: {GroupCount} grupos auditables con sus puntos auditables", result.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de grupos auditables");
                response.Messages.Add(new ApplicationMessage { Key = "Error", Message = ex.Message });
            }

            return response;
        }

        public async Task<ResponseDto<List<DataBySupervisorStoreResponseDto>>> GetDataBySupervisorStoreAsync(DataBySupervisorStoreRequestDto request)
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

                // Filtrar por StoreIds si se proporciona
                if (request.StoreIds != null && request.StoreIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.StoreId.HasValue && request.StoreIds.Contains(x.StoreId.Value));
                }

                // Filtrar por fecha de inicio si se proporciona
                if (request.StartDate.HasValue)
                {
                    baseFilter = baseFilter.AndAlso(x => x.StartDate >= request.StartDate.Value);
                }

                // Filtrar por fecha de fin si se proporciona
                if (request.EndDate.HasValue)
                {
                    var endDate = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    baseFilter = baseFilter.AndAlso(x => x.StartDate <= endDate);
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

                // Filtrar por UnitManagerIds si se proporciona
                if (request.UnitManagerIds != null && request.UnitManagerIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.UnitManager.Code
                        && request.UnitManagerIds.Contains(pap.UserReferenceId)));
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

                // Extraer datos por supervisor, tienda y mes
                var supervisorStoreData = periodAudits
                    .SelectMany(pa => pa.PeriodAuditParticipants
                        .Where(pap => pap.IsActive && pap.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code)
                        .Select(pap => new
                        {
                            pa.StoreId,
                            StoreName = pa.Store?.Name,
                            StoreCode = pa.Store?.Code,
                            SupervisorId = pap.UserReferenceId,
                            Month = pa.StartDate.Month,
                            Score = pa.ScoreValue
                        }))
                    .ToList();

                // Agrupar y calcular promedios por supervisor, tienda y mes
                var groupedData = supervisorStoreData
                    .GroupBy(x => new
                    {
                        x.SupervisorId,
                        x.StoreId,
                        x.StoreName,
                        x.StoreCode,
                        x.Month
                    })
                    .Select(g => new
                    {
                        g.Key.SupervisorId,
                        g.Key.StoreId,
                        g.Key.StoreName,
                        g.Key.StoreCode,
                        g.Key.Month,
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
                    Month = x.Month.ToString("00"),
                    Average = x.Average,
                    AuditCount = x.AuditCount
                })
                .OrderBy(x => x.SupervisorName)
                .ThenBy(x => x.Month)
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

        public async Task<ResponseDto<List<DataByStoreResponseDto>>> GetDataByStoreAsync(DataByStoreRequestDto request)
        {
            var response = ResponseDto.Create<List<DataByStoreResponseDto>>();
            try
            {
                _logger.LogInformation("Obteniendo calificaciones por tienda");

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

                // Filtrar por fecha de inicio si se proporciona
                if (request.StartDate.HasValue)
                {
                    baseFilter = baseFilter.AndAlso(x => x.StartDate >= request.StartDate.Value);
                }

                // Filtrar por fecha de fin si se proporciona
                if (request.EndDate.HasValue)
                {
                    var endDate = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    baseFilter = baseFilter.AndAlso(x => x.StartDate <= endDate);
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

                // Filtrar por UnitManagerIds si se proporciona
                if (request.UnitManagerIds != null && request.UnitManagerIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.UnitManager.Code
                        && request.UnitManagerIds.Contains(pap.UserReferenceId)));
                }

                // Obtener auditorías
                var periodAudits = await _periodAuditRepository.GetAsync(
                    filter: baseFilter,
                    includeProperties: [x => x.Store]);

                // Agrupar por tienda y calcular promedio
                var storeData = periodAudits
                    .GroupBy(pa => new
                    {
                        pa.StoreId,
                        StoreName = pa.Store?.Name
                    })
                    .Select(g => new DataByStoreResponseDto
                    {
                        StoreId = g.Key.StoreId?.ToString() ?? "",
                        StoreName = g.Key.StoreName ?? "",
                        Average = Math.Round(g.Average(x => x.ScoreValue), 2),
                        AuditCount = g.Count()
                    })
                    .OrderByDescending(x => x.Average)
                    .ToList();

                response.Data = storeData;
                _logger.LogInformation("Se generaron {Count} registros en el reporte por tienda", storeData.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener calificaciones por tienda");
                response.Messages.Add(new ApplicationMessage { Key = "Error", Message = ex.Message });
            }

            return response;
        }

        public async Task<ResponseDto<List<DataByScaleResponseDto>>> GetDataByScaleAsync(DataByScaleRequestDto request)
        {
            var response = ResponseDto.Create<List<DataByScaleResponseDto>>();
            try
            {
                _logger.LogInformation("Obteniendo promedios por escalas");

                // Obtener las escalas de la compañía por EnterpriseGroupingId
                var scaleCompanies = await _scaleCompanyRepository.GetAsync(filter: x => x.IsActive && x.EnterpriseGroupingId == request.EnterpriseGroupingId);

                if (!scaleCompanies.Any())
                {
                    response.Messages.Add(new ApplicationMessage { Key = "ValidationError", Message = "No se encontraron escalas configuradas para este agrupamiento empresarial" });
                    response.Data = new List<DataByScaleResponseDto>();
                    return response;
                }

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

                // Filtrar por fecha de inicio si se proporciona
                if (request.StartDate.HasValue)
                {
                    baseFilter = baseFilter.AndAlso(x => x.StartDate >= request.StartDate.Value);
                }

                // Filtrar por fecha de fin si se proporciona
                if (request.EndDate.HasValue)
                {
                    var endDate = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    baseFilter = baseFilter.AndAlso(x => x.StartDate <= endDate);
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

                // Filtrar por UnitManagerIds si se proporciona
                if (request.UnitManagerIds != null && request.UnitManagerIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.UnitManager.Code
                        && request.UnitManagerIds.Contains(pap.UserReferenceId)));
                }

                // Obtener auditorías
                var periodAudits = await _periodAuditRepository.GetAsync(
                    filter: baseFilter,
                    includeProperties: [x => x.Store]);

                if (!periodAudits.Any())
                {
                    response.Data = new List<DataByScaleResponseDto>();
                    return response;
                }

                // Clasificar cada auditoría en su escala según el promedio y agrupar por escala y mes
                var auditsByScale = periodAudits
                    .Select(pa => new
                    {
                        Audit = pa,
                        Month = pa.StartDate.Month,
                        Scale = scaleCompanies.FirstOrDefault(sc => 
                            pa.ScoreValue >= sc.MinValue && pa.ScoreValue <= sc.MaxValue)
                    })
                    .Where(x => x.Scale != null)
                    .GroupBy(x => x.Scale!.Name)
                    .Select(scaleGroup => new DataByScaleResponseDto
                    {
                        ScaleName = scaleGroup.Key,
                        Average = Math.Round(scaleGroup.Average(x => x.Audit.ScoreValue), 2),
                        AuditCount = scaleGroup.Count(),
                        Months = scaleGroup
                            .GroupBy(x => x.Month)
                            .Select(monthGroup => new MonthlyScaleDataDto
                            {
                                Month = monthGroup.Key.ToString("00"),
                                Average = Math.Round(monthGroup.Average(x => x.Audit.ScoreValue), 2),
                                AuditCount = monthGroup.Count()
                            })
                            .OrderBy(m => m.Month)
                            .ToArray()
                    })
                    .OrderBy(x => x.ScaleName)
                    .ToList();

                response.Data = auditsByScale;
                _logger.LogInformation("Se generaron {Count} registros en el reporte por escalas", auditsByScale.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener promedios por escalas");
                response.Messages.Add(new ApplicationMessage { Key = "Error", Message = ex.Message });
            }

            return response;
        }

        public async Task<ResponseDto<DataByFilterGeneralResponseDto>> GetGeneralByKPIsFilterAsync(DataByFilterGeneralRequestDto request)
        {
            var response = ResponseDto.Create<DataByFilterGeneralResponseDto>();
            try
            {
                var loggerPrefix = $"Obteniendo KPIs generales por filtro - EnterpriseIds: {(request.StoreIds != null ? string.Join(",", request.StoreIds) : "N/A")}, StoreIds: {(request.StoreIds != null ? string.Join(",", request.StoreIds) : "N/A")}, SupervisorIds: {(request.SupervisorIds != null ? string.Join(",", request.SupervisorIds) : "N/A")}, UnitManagerIds: {(request.UnitManagerIds != null ? string.Join(",", request.UnitManagerIds) : "N/A")}, StartDate: {request.StartDate?.ToString("yyyy-MM-dd") ?? "N/A"}, EndDate: {request.EndDate?.ToString("yyyy-MM-dd") ?? "N/A"}";
                _logger.LogInformation(loggerPrefix);

                Expression<Func<PeriodAudit, bool>> baseFilter = x => x.IsActive
                    && x.AuditStatus != null && x.AuditStatus.Code == AuditStatusCode.Completed;

                if (request.StoreIds != null && request.StoreIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.StoreId.HasValue && request.StoreIds.Contains(x.StoreId.Value));
                }

                if (request.SupervisorIds != null && request.SupervisorIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code
                        && request.SupervisorIds.Contains(pap.UserReferenceId)));
                }

                if (request.UnitManagerIds != null && request.UnitManagerIds.Length > 0)
                {
                    baseFilter = baseFilter.AndAlso(x => x.PeriodAuditParticipants.Any(pap =>
                        pap.IsActive
                        && pap.RoleCodeSnapshot == RoleCodes.UnitManager.Code
                        && request.UnitManagerIds.Contains(pap.UserReferenceId)));
                }

                if (request.StartDate.HasValue)
                {
                    baseFilter = baseFilter.AndAlso(x => x.StartDate >= request.StartDate.Value);
                }

                if (request.EndDate.HasValue)
                {
                    var endDate = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    baseFilter = baseFilter.AndAlso(x => x.StartDate <= endDate);
                }

                var periodAudits = await _periodAuditRepository.GetAsync(
                    filter: baseFilter,
                    includeProperties: [x => x.Store]);

                var quantityAudits = periodAudits.Count();
                var overallAverageScore = quantityAudits > 0 ? Math.Round(periodAudits.Average(x => x.ScoreValue), 2) : 0;
                var storesAudited = periodAudits.Select(x => x.StoreId).Distinct().Count();
                var percentageEvaluations = Convert.ToDecimal(quantityAudits > 0 ? Math.Round((double)quantityAudits / (storesAudited > 0 ? storesAudited : 1), 2) * 100 : 0);

                response.Data = new DataByFilterGeneralResponseDto
                {
                    QuantityAudits = quantityAudits,
                    OverallAverageScore = overallAverageScore,
                    QuantityStores = storesAudited,
                    PercentageEvaluations = percentageEvaluations
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener KPIs generales por filtro");
                response.Messages.Add(new ApplicationMessage { Key = "Error", Message = ex.Message });
            }
            return response;
        }
    }
}
