using System.Linq.Expressions;
using System.Text.Json;
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

        public async Task<ResponseDto<DataByFilterGeneralResponseDto>> GetGeneralKPIsAsync(DataByFilterGeneralRequestDto request)
        {
            var response = ResponseDto.Create<DataByFilterGeneralResponseDto>();
            try
            {
                var loggerPrefix = $"Obteniendo KPIs generales por filtro - EnterpriseIds: {(request.StoreIds != null ? string.Join(",", request.StoreIds) : "N/A")}, StoreIds: {(request.StoreIds != null ? string.Join(",", request.StoreIds) : "N/A")}, SupervisorIds: {(request.SupervisorIds != null ? string.Join(",", request.SupervisorIds) : "N/A")}, UnitManagerIds: {(request.UnitManagerIds != null ? string.Join(",", request.UnitManagerIds) : "N/A")}, StartDate: {request.StartDate?.ToString("yyyy-MM-dd") ?? "N/A"}, EndDate: {request.EndDate?.ToString("yyyy-MM-dd") ?? "N/A"}";
                _logger.LogInformation(loggerPrefix);

                Expression<Func<PeriodAudit, bool>> baseFilter = x => x.IsActive
                    && x.AuditStatus != null && x.AuditStatus.Code == AuditStatusCode.Completed;

                if (request.EnterpriseGrouping != Guid.Empty)
                {
                    baseFilter = baseFilter.AndAlso(x => x.Store.Enterprise.EnterpriseGroups.Any(eg => eg.EnterpriseGroupingId == request.EnterpriseGrouping && eg.IsActive));
                }

                if (request.EnterpriseIds != null)
                {
                    baseFilter = baseFilter.AndAlso(x => request.EnterpriseIds.Contains(x.Store.EnterpriseId));
                }

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
                            Month = pa.StartDate.Month,
                            pap.UserReferenceId,
                            Score = pa.ScoreValue
                        }))
                    .ToList();

                // Agrupar y calcular promedios por participante
                var groupedData = participantsData
                    .GroupBy(x => x.UserReferenceId)
                    .Select(participantGroup =>
                    {
                        var participantAverage = Math.Round(participantGroup.Average(x => x.Score), 2);

                        // Agrupar por mes dentro de cada participante
                        var monthlyData = participantGroup
                            .GroupBy(x => x.Month)
                            .Select(monthGroup =>
                            {
                                var monthAverage = Math.Round(monthGroup.Average(x => x.Score), 2);

                                return new
                                {
                                    Month = monthGroup.Key.ToString("00"),
                                    Average = monthAverage,
                                    AuditCount = monthGroup.Count()
                                };
                            })
                            .OrderBy(m => m.Month)
                            .ToList();

                        return new
                        {
                            UserReferenceId = participantGroup.Key,
                            Average = participantAverage,
                            AuditCount = participantGroup.Count(),
                            Months = monthlyData
                        };
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
                    UserReferenceId = x.UserReferenceId.ToString(),
                    UserName = userReferenceDict.TryGetValue(x.UserReferenceId, out var userRef)
                        ? $"{userRef.FirstName} {userRef.LastName}".Trim()
                        : "",
                    Average = x.Average,
                    AuditCount = x.AuditCount,
                    Months = x.Months.Select(m => new MonthlyParticipantDataDto
                    {
                        Month = m.Month,
                        Average = m.Average,
                        AuditCount = m.AuditCount
                    }).ToArray()
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

                // Obtener las escalas de la compañía por EnterpriseGroupingId
                var scaleCompanies = await _scaleCompanyRepository.GetAsync(filter: x => x.IsActive && x.EnterpriseGroupingId == request.EnterpriseGroupingId);

                if (!scaleCompanies.Any())
                {
                    response.Messages.Add(new ApplicationMessage { Key = "ValidationError", Message = "No se encontraron escalas configuradas para este agrupamiento empresarial" });
                    response.Data = new List<DataByAuditableGroupResponseDto>();
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
                            .Select(scaleData =>
                            {
                                var average = Math.Round(scaleData.Average(s => s.ScoreValue), 2);
                                var scale = scaleCompanies.FirstOrDefault(sc => 
                                    average > (sc.MinValue - 1) && average <= sc.MaxValue);

                                return new AuditablePointsResponseDto
                                {
                                    Code = scaleData.Key.ScaleCode ?? "",
                                    Name = scaleData.Key.ScaleName ?? "",
                                    Average = average,
                                    AuditCount = scaleData.Count(),
                                    RiskLevel = scale?.Name ?? "",
                                    RiskColor = scale?.ColorCode ?? ""
                                };
                            })
                            .OrderByDescending(ap => ap.Average)
                            .ToArray();

                        var groupAverage = Math.Round(groupData.Average(x => x.ScoreValue), 2);
                        var groupScale = scaleCompanies.FirstOrDefault(sc => 
                            groupAverage > (sc.MinValue - 1) && groupAverage <= sc.MaxValue);

                        return new DataByAuditableGroupResponseDto
                        {
                            Code = groupData.Key.GroupCode ?? "",
                            Name = groupData.Key.GroupName ?? "",
                            Average = groupAverage,
                            AuditCount = groupData.Count(),
                            RiskLevel = groupScale?.Name ?? "",
                            RiskColor = groupScale?.ColorCode ?? "",
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
                    StoreName = x.StoreName ?? "",
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

                // Obtener las escalas de la compañía por EnterpriseGroupingId
                var scaleCompanies = await _scaleCompanyRepository.GetAsync(filter: x => x.IsActive && x.EnterpriseGroupingId == request.EnterpriseGroupingId);

                if (!scaleCompanies.Any())
                {
                    response.Messages.Add(new ApplicationMessage { Key = "ValidationError", Message = "No se encontraron escalas configuradas para este agrupamiento empresarial" });
                    response.Data = new List<DataByStoreResponseDto>();
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

                // Agrupar por tienda y calcular promedio general y datos por mes
                var storeData = periodAudits
                    .GroupBy(pa => new
                    {
                        pa.StoreId,
                        StoreName = pa.Store?.Name
                    })
                    .Select(storeGroup =>
                    {
                        var storeAverage = Math.Round(storeGroup.Average(x => x.ScoreValue), 2);
                        var storeScale = scaleCompanies.FirstOrDefault(sc => 
                            storeAverage > (sc.MinValue - 1) && storeAverage <= sc.MaxValue);

                        // Agrupar por mes dentro de cada tienda
                        var monthlyData = storeGroup
                            .GroupBy(pa => pa.StartDate.Month)
                            .Select(monthGroup =>
                            {
                                var monthAverage = Math.Round(monthGroup.Average(x => x.ScoreValue), 2);
                                var monthScale = scaleCompanies.FirstOrDefault(sc => 
                                    monthAverage > (sc.MinValue - 1) && monthAverage <= sc.MaxValue);

                                return new MonthlyStoreDataDto
                                {
                                    Month = monthGroup.Key.ToString("00"),
                                    Average = monthAverage,
                                    AuditCount = monthGroup.Count(),
                                    RiskLevel = monthScale?.Name ?? "",
                                    RiskColor = monthScale?.ColorCode ?? ""
                                };
                            })
                            .OrderBy(m => m.Month)
                            .ToArray();

                        return new DataByStoreResponseDto
                        {
                            StoreId = storeGroup.Key.StoreId?.ToString() ?? "",
                            StoreName = storeGroup.Key.StoreName ?? "",
                            Average = storeAverage,
                            AuditCount = storeGroup.Count(),
                            RiskLevel = storeScale?.Name ?? "",
                            RiskColor = storeScale?.ColorCode ?? "",
                            Months = monthlyData
                        };
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
                            pa.ScoreValue > (sc.MinValue - 1) && pa.ScoreValue <= sc.MaxValue)
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

        public async Task<ResponseDto<List<DataExpirationResponseDto>>> GetExpiredProductsAsync(DataExpirationRequestDto request)
        {
            var response = ResponseDto.Create<List<DataExpirationResponseDto>>();
            try
            {
                var config = await _systemConfigurationRepository.GetFirstOrDefaultAsync(x => x.IsActive && x.ConfigKey == SystemConfigKey.ExpiredProductConfig.Code);

                string scaleGroupCode = "INV-5";
                string tableCode = "inv";
                string fieldCodeInsumo = "insumo";
                string fieldCodeCondProducto = "cond_producto";
                string fieldCodeCost = "cost";

                if (config != null)
                {
                    // configvalue = {"scaleGroup": "INV-5", "table": "inv", "productField": "insumo", "conditionField": "cond_producto", "costField": "cost"}
                    var configValues = JsonSerializer.Deserialize<Dictionary<string, string>>(config.ConfigValue);
                    if (configValues != null)
                    {
                        if (configValues.TryGetValue("scaleGroup", out var configScaleGroup))
                        {
                            scaleGroupCode = configScaleGroup;
                        }
                        if (configValues.TryGetValue("table", out var configTable))
                        {
                            tableCode = configTable;
                        }
                        if (configValues.TryGetValue("productField", out var configProductField))
                        {
                            fieldCodeInsumo = configProductField;
                        }
                        if (configValues.TryGetValue("conditionField", out var configConditionField))
                        {
                            fieldCodeCondProducto = configConditionField;
                        }
                        if (configValues.TryGetValue("costField", out var configCostField))
                        {
                            fieldCodeCost = configCostField;
                        }
                    }
                }
                
                _logger.LogInformation("Obteniendo productos próximos a vencer");

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

                // Filtrar solo auditorías que tengan un punto auditable con código "INV-5"
                baseFilter = baseFilter.AndAlso(x => x.PeriodAuditGroupResults.Any(gr =>
                    gr.IsActive &&
                    gr.PeriodAuditScaleResults.Any(sr =>
                        sr.IsActive &&
                        sr.ScaleGroup != null &&
                        sr.ScaleGroup.Code == scaleGroupCode &&
                        sr.PeriodAuditTableScaleTemplateResults.Any(t => t.IsActive && t.Code == tableCode)
                    )
                ));

                var audits = await _periodAuditRepository.GetAsync(
                    filter: baseFilter,
                    includeProperties: [
                        x => x.Store,
                        x => x.PeriodAuditParticipants
                    ]);

                if (!audits.Any())
                {
                    response.Data = new List<DataExpirationResponseDto>();
                    _logger.LogInformation("No se encontraron auditorías con productos");
                    return response;
                }

                var auditIds = audits.Select(x => x.PeriodAuditId).ToList();

                // Obtener información de supervisores
                var supervisorParticipants = audits
                    .SelectMany(a => a.PeriodAuditParticipants
                        .Where(p => p.IsActive && p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code)
                        .Select(p => new { a.PeriodAuditId, p.UserReferenceId }))
                    .GroupBy(x => x.PeriodAuditId)
                    .ToDictionary(g => g.Key, g => g.First().UserReferenceId);

                var supervisorIds = supervisorParticipants.Values.Distinct().ToList();
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

                // Obtener las tablas con el punto auditable INV-5
                var tables = await _periodAuditTableScaleTemplateResultRepository.GetAsync(
                    filter: t => t.IsActive
                        && auditIds.Contains(t.PeriodAuditScaleResult!.PeriodAuditGroupResult!.PeriodAuditId)
                        && t.PeriodAuditScaleResult!.ScaleGroup!.Code == scaleGroupCode && t.Code == tableCode,
                    includeProperties:
                    [
                        t => t.PeriodAuditScaleResult!.PeriodAuditGroupResult!.PeriodAudit!.Store,
                        t => t.PeriodAuditScaleResult.ScaleGroup!,
                        t => t.PeriodAuditFieldValues!
                    ]);

                // Procesar los datos horizontales
                var result = new List<DataExpirationResponseDto>();

                foreach (var table in tables)
                {
                    var audit = table.PeriodAuditScaleResult?.PeriodAuditGroupResult?.PeriodAudit;
                    
                    // Obtener información del supervisor
                    var supervisorId = supervisorParticipants.TryGetValue(audit?.PeriodAuditId ?? Guid.Empty, out var supId) ? supId : Guid.Empty;
                    var supervisorName = supervisorDict.TryGetValue(supervisorId, out var supervisor)
                        ? $"{supervisor.FirstName} {supervisor.LastName}".Trim()
                        : "";
                    
                    // Preparar campos genéricos una sola vez por tabla
                    var auditDate = audit?.StartDate ?? DateTime.MinValue;
                    var auditId = audit?.PeriodAuditId.ToString() ?? "";
                    var storeName = audit?.Store?.Name ?? "";
                    var storeId = audit?.Store?.StoreId;
                    var year = auditDate.Year;
                    var month = auditDate.Month.ToString("D2");
                    var supervisorIdStr = supervisorId.ToString();
                    
                    // Obtener todos los campos con TableDataHorizontal
                    var fieldValues = table.PeriodAuditFieldValues?
                        .Where(f => f.IsActive 
                            && !string.IsNullOrEmpty(f.TableDataHorizontal)
                            && f.FieldCode != null)
                        .ToList();

                    if (fieldValues == null || !fieldValues.Any())
                        continue;

                    // Recopilar todas las filas de todos los campos
                    var rowsData = new Dictionary<int, Dictionary<string, object?>>();

                    foreach (var field in fieldValues)
                    {
                        try
                        {
                            var horizontalData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(field.TableDataHorizontal);

                            if (horizontalData != null && horizontalData.Any())
                            {
                                foreach (var rowData in horizontalData)
                                {
                                    var rowNumber = rowData.ContainsKey("row") ? ((JsonElement)rowData["row"]).GetInt32() : 0;

                                    // Crear el registro de la fila si no existe
                                    if (!rowsData.ContainsKey(rowNumber))
                                    {
                                        rowsData[rowNumber] = new Dictionary<string, object?>();
                                    }

                                    // Agregar el valor del campo usando el FieldCode como nombre de propiedad
                                    if (rowData.ContainsKey("value"))
                                    {
                                        var valueElement = rowData["value"];
                                        object? fieldValue = null;

                                        if (valueElement is JsonElement je)
                                        {
                                            fieldValue = je.ValueKind switch
                                            {
                                                JsonValueKind.String => je.GetString(),
                                                JsonValueKind.Number => je.TryGetInt32(out var intVal) ? (object)intVal : je.GetDouble(),
                                                JsonValueKind.True => true,
                                                JsonValueKind.False => false,
                                                JsonValueKind.Null => null,
                                                _ => je.ToString()
                                            };
                                        }
                                        else
                                        {
                                            fieldValue = valueElement;
                                        }

                                        // Usar el FieldCode como nombre de la propiedad
                                        rowsData[rowNumber][field.FieldCode!] = fieldValue;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error al procesar TableDataHorizontal para campo {FieldCode}", field.FieldCode);
                        }
                    }

                    // Agregar todas las filas al resultado con campos genéricos
                    foreach (var row in rowsData.OrderBy(r => r.Key))
                    {
                        var description = row.Value.ContainsKey(fieldCodeInsumo) ? row.Value[fieldCodeInsumo]?.ToString() : null;
                        var observation = row.Value.ContainsKey(fieldCodeCondProducto) ? row.Value[fieldCodeCondProducto]?.ToString() : null;
                        
                        decimal cost = 0m;
                        if (row.Value.ContainsKey(fieldCodeCost) && row.Value[fieldCodeCost] != null)
                        {
                            var costValue = row.Value[fieldCodeCost];
                            if (costValue is JsonElement costJe)
                            {
                                cost = costJe.ValueKind == JsonValueKind.Number 
                                    ? costJe.TryGetDecimal(out var decVal) ? decVal : (decimal)costJe.GetDouble()
                                    : decimal.TryParse(costJe.GetString(), out var strVal) ? strVal : 0m;
                            }
                            else if (decimal.TryParse(costValue.ToString(), out var parsedVal))
                            {
                                cost = parsedVal;
                            }
                        }

                        result.Add(new DataExpirationResponseDto
                        {
                            AuditId = auditId,
                            StoreName = storeName,
                            StoreId = storeId,
                            AuditDate = auditDate,
                            Year = year,
                            Month = month,
                            SupervisorId = supervisorIdStr,
                            SupervisorName = supervisorName,
                            Description = description,
                            Cost = cost,
                            Observation = observation
                        });
                    }
                }

                response.Data = result;
                _logger.LogInformation("Se encontraron {Count} registros de productos", result.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos próximos a vencer");
                response.Messages.Add(new ApplicationMessage { Key = "Error", Message = ex.Message });
            }

            return response;
        }
    }
}
