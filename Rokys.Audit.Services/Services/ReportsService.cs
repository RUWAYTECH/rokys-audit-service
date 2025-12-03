using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Common.Helpers;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Reports;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Requests.Reports;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.DTOs.Responses.AuditStatus;
using DocumentFormat.OpenXml.Spreadsheet;
using Rokys.Audit.Services.Services.ReportUtils;

namespace Rokys.Audit.Services.Services
{
    /// <summary>
    /// Servicio para generar reportes y datos de dashboard
    /// </summary>
    public class ReportsService : IReportsService
    {
        private readonly IPeriodAuditRepository _periodAuditRepository;
        private readonly ILogger<ReportsService> _logger;
        private readonly IAuditStatusRepository _auditStatus;
        private readonly IAMapper _mapper;
        private readonly IScaleCompanyRepository _scaleCompanyRepository;
        private readonly IPeriodAuditParticipantRepository _periodAuditParticipantRepository;

        public ReportsService(
            IPeriodAuditRepository periodAuditRepository,
            ILogger<ReportsService> logger,
            IAuditStatusRepository auditStatusRepository,
            IAMapper mapper,
            IScaleCompanyRepository scaleCompanyRepository,
            IPeriodAuditParticipantRepository periodAuditParticipantRepository)
        {
            _periodAuditRepository = periodAuditRepository;
            _logger = logger;
            _auditStatus = auditStatusRepository;
            _mapper = mapper;
            _periodAuditParticipantRepository = periodAuditParticipantRepository;
            _scaleCompanyRepository = scaleCompanyRepository;
        }

        public async Task<ResponseDto<DashboardDataResponseDto>> GetDashboardEvolutionsDataAsync(int year, Guid enterpriseId)
        {
            var response = ResponseDto.Create<DashboardDataResponseDto>();
            try
            {
                // Obtener auditorías del año especificado
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31, 23, 59, 59);

                var periodAudits = await _periodAuditRepository.GetAsync(
                    filter: x => x.CreationDate >= startDate &&
                    x.CreationDate <= endDate && x.IsActive &&
                    x.Store.EnterpriseId == enterpriseId
                    && x.AuditStatus != null && x.AuditStatus.Code == AuditStatusCode.Completed);

                // Crear las categorías (meses)
                var categories = new List<string>
                {
                    "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                };

                // Agrupar auditorías por ScaleCode y mes
                var auditsByScaleAndMonth = periodAudits
                    .Where(x => !string.IsNullOrEmpty(x.ScaleCode))
                    .GroupBy(x => new { x.ScaleCode, x.ScaleName, x.ScaleColor })
                    .ToList();

                // Crear series dinámicamente basadas en los ScaleCodes encontrados
                var series = new List<DashboardSeriesDto>();

                foreach (var scaleGroup in auditsByScaleAndMonth)
                {
                    var monthlyData = new List<decimal>();

                    // Calcular datos por mes para esta escala
                    for (int month = 1; month <= 12; month++)
                    {
                        var countForMonth = scaleGroup.Count(x => x.CreationDate.Month == month);
                        monthlyData.Add(countForMonth);
                    }

                    // Crear serie para esta escala
                    var seriesDto = new DashboardSeriesDto
                    {
                        Name = scaleGroup.Key.ScaleName ?? scaleGroup.Key.ScaleCode,
                        Type = "column",
                        Data = monthlyData,
                        Color = scaleGroup.Key.ScaleColor ?? "#999999" // Color por defecto si no tiene
                    };

                    series.Add(seriesDto);
                }

                // Agregar serie de ranking de desempeño si hay datos
                if (periodAudits.Any())
                {
                    var totalAuditsByMonth = new Dictionary<int, int>();
                    for (int month = 1; month <= 12; month++)
                    {
                        totalAuditsByMonth[month] = periodAudits.Count(x => x.CreationDate.Month == month);
                    }

                    series.Add(new DashboardSeriesDto
                    {
                        Name = "Ranking de desempeño",
                        Type = "spline",
                        YAxis = 1,
                        Color = "#004d99",
                        Data = CalculatePerformanceRanking(totalAuditsByMonth),
                        Tooltip = new DashboardTooltipDto { ValueSuffix = " pts" }
                    });
                }


                var dashboardData = new DashboardDataResponseDto
                {
                    Categories = categories,
                    Series = series
                };

                response.Data = dashboardData;

                _logger.LogInformation($"Dashboard data generated successfully for year {year}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating dashboard data for year {year}: {ex.Message}");
                response = ResponseDto.Error<DashboardDataResponseDto>(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Calcula el ranking de desempeño por mes
        /// </summary>
        private List<decimal> CalculatePerformanceRanking(Dictionary<int, int> auditsByMonth)
        {
            var data = new List<decimal>();
            var baseScore = 400m;

            for (int month = 1; month <= 12; month++)
            {
                var totalAudits = auditsByMonth[month];
                // El ranking aumenta con más auditorías realizadas
                var score = baseScore + (totalAudits * 5m) + (month * 2m); // Mejora progresiva
                data.Add(score);
            }
            return data;
        }

        public async Task<ResponseDto<DashboardDataResponseDto>> GetDashboardSupervisorsDataAsync(int year, Guid enterpriseId, Guid[] supervisorIds)
        {
            var response = ResponseDto.Create<DashboardDataResponseDto>();
            try
            {
                // Obtener auditorías del año especificado
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31, 23, 59, 59);

                // Filtro base para la empresa
                Expression<Func<PeriodAudit, bool>> baseFilter = x => x.CreationDate >= startDate &&
                                                                       x.CreationDate <= endDate &&
                                                                       x.IsActive &&
                                                                       x.Store.EnterpriseId == enterpriseId &&
                                                                       x.AuditStatus != null &&
                                                                       x.AuditStatus.Code == AuditStatusCode.Completed;

                // Obtener TODAS las auditorías de la empresa para el promedio general
                var allEnterpriseAudits = await _periodAuditRepository.GetCustomSearchAsync(baseFilter);

                // Filtrar por supervisores seleccionados
                Expression<Func<PeriodAudit, bool>> filter = baseFilter;
                if (supervisorIds != null && supervisorIds.Length > 0)
                {
                    var auditIdsWithSupervisor = await _periodAuditParticipantRepository.GetAsync(
                            filter: p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code &&
                                        supervisorIds.Contains(p.UserReferenceId)
                        );
                    filter = filter.AndAlso(x => auditIdsWithSupervisor.Select(a => a.PeriodAuditId).Contains(x.PeriodAuditId));
                }

                var periodAudits = await _periodAuditRepository.GetCustomSearchAsync(filter);

                // Crear las categorías (meses)
                var categories = new List<string>
                {
                    "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                };

                // Agrupar auditorías por supervisor
                var auditsBySupervisor = periodAudits
                    .Where(x => x.PeriodAuditParticipants.Any(p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code))
                    .GroupBy(x => x.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code)!.UserReferenceId)
                    .ToList();

                // Crear series dinámicamente por supervisor
                var series = new List<DashboardSeriesDto>();
                var dashStyles = new[] { "ShortDot", "Dash", "ShortDash", "LongDash", "DashDot" };

                var supervisorIndex = 0;
                foreach (var supervisorGroup in auditsBySupervisor)
                {
                    var supervisor = supervisorGroup.Key;
                    var supervisorName = supervisorGroup.First().PeriodAuditParticipants
                        .FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code)?.UserReference?.FullName ?? "Supervisor";

                    // Calcular cantidad de auditorías por mes
                    var monthlyCountData = new List<decimal>();
                    for (int month = 1; month <= 12; month++)
                    {
                        var countForMonth = supervisorGroup.Count(x => x.CreationDate.Month == month);
                        monthlyCountData.Add(countForMonth);
                    }

                    // Calcular promedio de calificación por mes
                    var monthlyAvgData = new List<decimal>();
                    for (int month = 1; month <= 12; month++)
                    {
                        var auditsInMonth = supervisorGroup.Where(x => x.CreationDate.Month == month).ToList();
                        if (auditsInMonth.Any())
                        {
                            var avgScore = auditsInMonth.Average(x => x.ScoreValue);
                            var roundedAvg = Math.Round(avgScore, 2);
                            monthlyAvgData.Add(roundedAvg);
                        }
                        else
                        {
                            monthlyAvgData.Add(0);
                        }
                    }

                    // Generar color único para cada supervisor
                    var color = supervisorIndex < Constants.ColorPalette.Palette.Length
                        ? Constants.ColorPalette.Palette[supervisorIndex]
                        : GenerateColor.GenerateColorFromIndex(supervisorIndex);

                    // Crear serie para cantidad de auditorías (columnas)
                    var countSeries = new DashboardSeriesDto
                    {
                        Name = $"{supervisorName} - Cantidad",
                        Type = "column",
                        YAxis = 1, // Eje secundario (derecha)
                        Data = monthlyCountData,
                        Color = color,
                        Tooltip = new DashboardTooltipDto { ValueSuffix = " auditorías" }
                    };

                    // Crear serie para promedio de calificación (línea)
                    var avgSeries = new DashboardSeriesDto
                    {
                        Name = $"{supervisorName} - Promedio",
                        Type = "spline",
                        YAxis = 0, // Eje principal (izquierda)
                        Data = monthlyAvgData,
                        Color = color,
                        DashStyle = supervisorIndex == 0 ? null : dashStyles[(supervisorIndex - 1) % dashStyles.Length],
                        Tooltip = new DashboardTooltipDto { ValueSuffix = " pts" }
                    };

                    series.Add(countSeries);
                    series.Add(avgSeries);
                    supervisorIndex++;
                }

                // Calcular y agregar la serie de promedio general de TODOS los supervisores de la empresa
                var allSupervisorAudits = allEnterpriseAudits
                    .Where(x => x.PeriodAuditParticipants.Any(p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code))
                    .ToList();

                if (allSupervisorAudits.Any())
                {
                    // Agrupar todas las auditorías de la empresa por supervisor
                    var allSupervisorsByMonth = allSupervisorAudits
                        .GroupBy(x => x.PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code)!.UserReferenceId)
                        .ToList();

                    var generalAverageData = new List<decimal>();
                    for (int month = 1; month <= 12; month++)
                    {
                        var monthlySupervisorAverages = new List<decimal>();

                        // Calcular el promedio de cada supervisor para este mes
                        foreach (var supervisorGroup in allSupervisorsByMonth)
                        {
                            var auditsInMonth = supervisorGroup.Where(x => x.CreationDate.Month == month).ToList();
                            if (auditsInMonth.Any())
                            {
                                var supervisorAvg = auditsInMonth.Average(x => x.ScoreValue);
                                monthlySupervisorAverages.Add(supervisorAvg);
                            }
                        }

                        // Calcular el promedio general del mes (promedio de promedios de supervisores)
                        if (monthlySupervisorAverages.Any())
                        {
                            var generalAvg = monthlySupervisorAverages.Average();
                            generalAverageData.Add(Math.Round(generalAvg, 2));
                        }
                        else
                        {
                            generalAverageData.Add(0);
                        }
                    }

                    // Crear serie para el promedio general en color rojo
                    var generalAvgSeries = new DashboardSeriesDto
                    {
                        Name = "Promedio General",
                        Type = "spline",
                        YAxis = 0, // Eje principal (izquierda)
                        Data = generalAverageData,
                        Color = "#FF0000", // Color rojo
                        DashStyle = "Solid",
                        Tooltip = new DashboardTooltipDto { ValueSuffix = " pts" },
                        LineWidth = 3 // Línea más gruesa para distinguirla
                    };

                    series.Add(generalAvgSeries);
                }

                var dashboardData = new DashboardDataResponseDto
                {
                    Categories = categories,
                    Series = series
                };

                response.Data = dashboardData;

                _logger.LogInformation($"Dashboard supervisors data generated successfully for year {year} with {supervisorIds?.Length ?? 0} supervisors");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating dashboard supervisors data for year {year}: {ex.Message}");
                response = ResponseDto.Error<DashboardDataResponseDto>(ex.Message);
            }

            return response;
        }

        public async Task<ResponseDto<DashboardDataResponseDto>> GetDashboardStoresDataAsync(int year, Guid enterpriseId, Guid[] storeIds)
        {
            var response = ResponseDto.Create<DashboardDataResponseDto>();
            try
            {
                // Obtener auditorías del año especificado y filtradas por tiendas
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31, 23, 59, 59);

                // Filtro base para la empresa
                Expression<Func<PeriodAudit, bool>> baseFilter = x => x.CreationDate >= startDate &&
                                                                       x.CreationDate <= endDate &&
                                                                       x.IsActive &&
                                                                       x.Store != null &&
                                                                       x.Store.EnterpriseId == enterpriseId &&
                                                                       x.AuditStatus != null &&
                                                                       x.AuditStatus.Code == AuditStatusCode.Completed;

                // Obtener TODAS las auditorías de la empresa para el promedio general
                var allEnterpriseAudits = await _periodAuditRepository.GetCustomSearchAsync(baseFilter);

                // Filtrar por tiendas seleccionadas para mostrar en el gráfico
                Expression<Func<PeriodAudit, bool>> filter = baseFilter;
                if (storeIds != null && storeIds.Length > 0)
                {
                    var storeIdsList = storeIds.ToList();
                    filter = filter.AndAlso(x => x.StoreId != null && storeIdsList.Contains(x.StoreId.Value));
                }

                var periodAudits = await _periodAuditRepository.GetCustomSearchAsync(filter);

                // Crear las categorías (meses)
                var categories = new List<string>
                {
                    "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                };

                // Agrupar auditorías por tienda
                var auditsByStore = periodAudits
                    .GroupBy(x => x.StoreId)
                    .ToList();

                // Crear series dinámicamente por tienda
                var series = new List<DashboardSeriesDto>();
                var dashStyles = new[] { "ShortDot", "Dash", "ShortDash", "LongDash", "DashDot" };

                var storeIndex = 0;
                foreach (var storeGroup in auditsByStore)
                {
                    var storeId = storeGroup.Key;
                    var storeName = (storeGroup.First().Store?.Name ?? "Tienda").Trim();

                    // Calcular cantidad de auditorías por mes
                    var monthlyCountData = new List<decimal>();
                    for (int month = 1; month <= 12; month++)
                    {
                        var countForMonth = storeGroup.Count(x => x.CreationDate.Month == month);
                        monthlyCountData.Add(countForMonth);
                    }

                    // Calcular promedio de calificación por mes
                    var monthlyAvgData = new List<decimal>();
                    for (int month = 1; month <= 12; month++)
                    {
                        var auditsInMonth = storeGroup.Where(x => x.CreationDate.Month == month).ToList();
                        if (auditsInMonth.Any())
                        {
                            var avgScore = auditsInMonth.Average(x => x.ScoreValue);
                            var roundedAvg = Math.Round(avgScore, 2);
                            monthlyAvgData.Add(roundedAvg);
                        }
                        else
                        {
                            monthlyAvgData.Add(0);
                        }
                    }

                    // Generar color único para cada tienda
                    var color = storeIndex < Constants.ColorPalette.Palette.Length
                        ? Constants.ColorPalette.Palette[storeIndex]
                        : GenerateColor.GenerateColorFromIndex(storeIndex);

                    // Crear serie para cantidad de auditorías (columnas)
                    var countSeries = new DashboardSeriesDto
                    {
                        Name = $"{storeName} - Cantidad",
                        Type = "column",
                        YAxis = 1, // Eje secundario (derecha)
                        Data = monthlyCountData,
                        Color = color,
                        Tooltip = new DashboardTooltipDto { ValueSuffix = " auditorías" }
                    };

                    // Crear serie para promedio de calificación (línea)
                    var avgSeries = new DashboardSeriesDto
                    {
                        Name = $"{storeName} - Promedio",
                        Type = "spline",
                        YAxis = 0, // Eje principal (izquierda)
                        Data = monthlyAvgData,
                        Color = color,
                        DashStyle = storeIndex == 0 ? null : dashStyles[(storeIndex - 1) % dashStyles.Length],
                        Tooltip = new DashboardTooltipDto { ValueSuffix = " pts" }
                    };

                    series.Add(countSeries);
                    series.Add(avgSeries);
                    storeIndex++;
                }

                // Calcular y agregar la serie de promedio general de TODAS las tiendas de la empresa
                if (allEnterpriseAudits.Any())
                {
                    // Agrupar todas las auditorías de la empresa por tienda y mes
                    var allStoresByMonth = allEnterpriseAudits.GroupBy(x => x.StoreId).ToList();

                    var generalAverageData = new List<decimal>();
                    for (int month = 1; month <= 12; month++)
                    {
                        var monthlyStoreAverages = new List<decimal>();

                        // Calcular el promedio de cada tienda para este mes
                        foreach (var storeGroup in allStoresByMonth)
                        {
                            var auditsInMonth = storeGroup.Where(x => x.CreationDate.Month == month).ToList();
                            if (auditsInMonth.Any())
                            {
                                var storeAvg = auditsInMonth.Average(x => x.ScoreValue);
                                monthlyStoreAverages.Add(storeAvg);
                            }
                        }

                        // Calcular el promedio general del mes (promedio de promedios de tiendas)
                        if (monthlyStoreAverages.Any())
                        {
                            var generalAvg = monthlyStoreAverages.Average();
                            generalAverageData.Add(Math.Round(generalAvg, 2));
                        }
                        else
                        {
                            generalAverageData.Add(0);
                        }
                    }

                    // Crear serie para el promedio general en color rojo
                    var generalAvgSeries = new DashboardSeriesDto
                    {
                        Name = "Promedio General",
                        Type = "spline",
                        YAxis = 0, // Eje principal (izquierda)
                        Data = generalAverageData,
                        Color = "#FF0000", // Color rojo
                        DashStyle = "Solid",
                        Tooltip = new DashboardTooltipDto { ValueSuffix = " pts" },
                        LineWidth = 3 // Línea más gruesa para distinguirla
                    };

                    series.Add(generalAvgSeries);
                }

                var dashboardData = new DashboardDataResponseDto
                {
                    Categories = categories,
                    Series = series
                };

                response.Data = dashboardData;

                _logger.LogInformation($"Dashboard stores data generated successfully for year {year} with {storeIds?.Length ?? 0} stores");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating dashboard stores data for year {year}: {ex.Message}");
                response = ResponseDto.Error<DashboardDataResponseDto>(ex.Message);
            }

            return response;
        }

        public async Task<ResponseDto<PeriodAuditReportResponseDto>> GetReportSearchAsync(ReportSearchFilterRequestDto reportSearchFilterRequestDto)
        {
            var response = ResponseDto.Create<PeriodAuditReportResponseDto>();
            try
            {
                Expression<Func<PeriodAudit, bool>> filter = x => x.IsActive;

                if (reportSearchFilterRequestDto.StoreId.HasValue)
                    filter = filter.AndAlso(x => x.StoreId == reportSearchFilterRequestDto.StoreId.Value && x.IsActive);

                if (reportSearchFilterRequestDto.EnterpriseId.HasValue)
                    filter = filter.AndAlso(x => x.Store.EnterpriseId == reportSearchFilterRequestDto.EnterpriseId.Value && x.IsActive);

                if (reportSearchFilterRequestDto.ResponsibleAuditorId.HasValue)
                    filter = filter.AndAlso(x => x.PeriodAuditParticipants.Any(p => p.UserReferenceId == reportSearchFilterRequestDto.ResponsibleAuditorId.Value && p.RoleCodeSnapshot == RoleCodes.Auditor.Code) && x.IsActive);

                if (reportSearchFilterRequestDto.SupervisorId.HasValue)
                    filter = filter.AndAlso(x => x.PeriodAuditParticipants.Any(p => p.UserReferenceId == reportSearchFilterRequestDto.SupervisorId.Value && p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code) && x.IsActive);

                // Filtro por rango de fechas
                if (reportSearchFilterRequestDto.ReportDateInit.HasValue && reportSearchFilterRequestDto.ReportDateFinish.HasValue)
                {
                    // Si se proporcionan ambas fechas, usar rango completo
                    var startDate = reportSearchFilterRequestDto.ReportDateInit.Value.Date; // Inicio del día
                    var endDate = reportSearchFilterRequestDto.ReportDateFinish.Value.Date.AddDays(1).AddTicks(-1); // Fin del día

                    filter = filter.AndAlso(x =>
                        x.CreationDate >= startDate &&
                        x.CreationDate <= endDate &&
                        x.IsActive
                    );
                }
                else if (reportSearchFilterRequestDto.ReportDateInit.HasValue)
                {
                    // Si solo se proporciona fecha inicial, desde esa fecha en adelante
                    var startDate = reportSearchFilterRequestDto.ReportDateInit.Value.Date;
                    filter = filter.AndAlso(x => x.CreationDate >= startDate && x.IsActive);
                }
                else if (reportSearchFilterRequestDto.ReportDateFinish.HasValue)
                {
                    // Si solo se proporciona fecha final, hasta esa fecha
                    var endDate = reportSearchFilterRequestDto.ReportDateFinish.Value.Date.AddDays(1).AddTicks(-1);
                    filter = filter.AndAlso(x => x.CreationDate <= endDate && x.IsActive);
                }
                else if (!string.IsNullOrEmpty(reportSearchFilterRequestDto.ReportDate))
                {
                    // Mantener compatibilidad con ReportDate antiguo (por mes completo)
                    // Soporta formatos: "YYYY-MM" o "YYYY-MM-DD"
                    DateTime parsedDate;

                    if (DateTime.TryParse(reportSearchFilterRequestDto.ReportDate, out parsedDate))
                    {
                        var startDate = new DateTime(parsedDate.Year, parsedDate.Month, 1, 0, 0, 0);
                        var endDate = startDate.AddMonths(1).AddTicks(-1);

                        filter = filter.AndAlso(x =>
                            x.CreationDate >= startDate &&
                            x.CreationDate <= endDate &&
                            x.IsActive
                        );
                    }
                }

                filter = filter.AndAlso(x => x.AuditStatus.Code == AuditStatusCode.Completed);

          
                var entities = await _periodAuditRepository.GetCustomSearchAsync(
                    filter: filter
                );

                IEnumerable<ScaleCompany> scaleCompanies = [];

                if (reportSearchFilterRequestDto.EnterpriseId.HasValue)
                {
                    scaleCompanies = await _scaleCompanyRepository.GetAsync(
                        filter: x => x.EnterpriseId == reportSearchFilterRequestDto.EnterpriseId.Value && x.IsActive
                    );
                }


                var storeGroups = entities.GroupBy(e => e.StoreId);

                var itemDtos = new List<PeriodAuditItemReportResponseDto>();
                foreach (var group in storeGroups)
                {

                    var storeEntity = group.FirstOrDefault();
                    if (storeEntity == null)
                        continue;
                      

                    var totalAudits = group.Count();
                    var totalScore = group.Sum(x => x.ScoreValue);
                    decimal averageScore = totalAudits > 0 ? totalScore / totalAudits : 0;
                    IEnumerable<ScaleCompany> currentScales = scaleCompanies;

                    if (currentScales == null || !currentScales.Any())
                    {
                        currentScales = await _scaleCompanyRepository.GetAsync(
                            filter: x => x.EnterpriseId == storeEntity.Store.EnterpriseId || x.EnterpriseId == null && x.IsActive
                        );
                    }
                    string riskLevel = "Sin Escala";
                    string riskColor = "#FFFFFF";
                    foreach (var scale in currentScales)
                    {
                        if (averageScore >= scale.MinValue && averageScore <= scale.MaxValue)
                        {
                            riskLevel = scale.Name;
                            riskColor = scale.ColorCode ?? "#FFFFFF";
                            break;
                        }
                    }

                    var dto = new PeriodAuditItemReportResponseDto
                    {
                        StoreId = storeEntity.StoreId,
                        StoreName = storeEntity.Store?.Name ?? string.Empty,
                        EnterpriseId = storeEntity.Store?.EnterpriseId,
                        EnterpriseName = storeEntity.Store?.Enterprise?.Name ?? string.Empty,
                        AuditedQuantityPerStore = totalAudits,
                        Ranking = null, // se puede calcular luego si hay un ranking general
                        MothlyScore = Math.Round(averageScore, 2),
                        LevelRisk = riskLevel,
                        RiskColor = riskColor,
                        Auditor = group
                            .SelectMany(x => x.PeriodAuditParticipants)
                            .Where(p => p.RoleCodeSnapshot == RoleCodes.Auditor.Code && p.UserReference != null)
                            .GroupBy(p => p.UserReference.FullName)
                            .Select(g => new UserInfoAuditItem
                            {
                                FullName = g.Key,
                                TotalAudits = g.Count()
                            })
                            .ToList(),

                        Supervisor = group
                            .SelectMany(x => x.PeriodAuditParticipants)
                            .Where(p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code && p.UserReference != null)
                            .GroupBy(p => p.UserReference.FullName)
                            .Select(g => new UserInfoAuditItem
                            {
                                FullName = g.Key,
                                TotalAudits = g.Count()
                            })
                            .ToList(),

                        OperationManager = group
                            .SelectMany(x => x.PeriodAuditParticipants)
                            .Where(p => p.RoleCodeSnapshot == RoleCodes.JefeDeOperaciones.Code && p.UserReference != null)
                            .GroupBy(p => p.UserReference.FullName)
                            .Select(g => new UserInfoAuditItem
                            {
                                FullName = g.Key,
                                TotalAudits = g.Count()
                            })
                            .ToList(),
                        AuditStatus = _mapper.Map<AuditStatusResponseDto>(storeEntity.AuditStatus)
                    };

                    itemDtos.Add(dto);
                }

                var rankedItems = itemDtos
                    .OrderByDescending(x => x.MothlyScore)
                    .ThenBy(x => x.StoreName)
                    .ToList();
                int rank = 1;
                foreach (var item in rankedItems)
                {
                    item.Ranking = rank++;
                }

                // Reasignar la lista ordenada
                itemDtos = rankedItems;

                if (!scaleCompanies.Any())
                {
                    scaleCompanies = await _scaleCompanyRepository.GetAsync(
                                filter: x => x.EnterpriseId == null && x.IsActive
                    );
                }

                var globalAverage = itemDtos.Any() ? itemDtos.Average(x => x.MothlyScore) : 0m;
                string globalRiskLevel = "Sin Escala";
                string globalRiskColor = "#FFFFFF";
                foreach (var scale in scaleCompanies)
                {
                    if (globalAverage >= scale.MinValue && globalAverage <= scale.MaxValue)
                    {
                        globalRiskLevel = scale.Name;
                        globalRiskColor = scale.ColorCode ?? "#FFFFFF";
                        break;
                    }
                }

                int globalRank = 0;
                if (itemDtos.Any())
                {
                    var ordered = itemDtos.OrderByDescending(x => x.MothlyScore).ToList();

                    for (int i = 0; i < ordered.Count; i++)
                    {
                        var score = ordered[i].MothlyScore;
                        if (globalAverage >= score)
                        {
                            globalRank = i;
                            break;
                        }
                    }
                    if (globalRank == 0)
                        globalRank = ordered.Count;
                }

                var dataResult = new PeriodAuditReportResponseDto
                {
                    Items = itemDtos,

                    Summaries = new List<SummaryReportResponseDto>{ new SummaryReportResponseDto
                    {
                        Ranking = globalRank,
                        ResultByMonth = Math.Round(globalAverage,2),
                        Risk = globalRiskLevel,
                        RiskColor = globalRiskColor,
                        QuantityAudit = entities.Count
                    } }

                };


                response.Data = dataResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditReportResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<ExportReportResultDto>> ExportReport(ReportSearchFilterRequestDto reportSearchFilterRequestDto)
        {
            var response = ResponseDto.Create<ExportReportResultDto>();
            try
            {
                var reportDataResponse = await GetReportSearchAsync(reportSearchFilterRequestDto);
                if (!reportDataResponse.IsValid || reportDataResponse.Data == null)
                {
                    return ResponseDto.Error<ExportReportResultDto>("No se pudieron obtener los datos del reporte para exportar.");
                }

                // Aquí se implementaría la lógica de exportación, por ejemplo a Excel o PDF.
                // Por simplicidad, asumiremos que se genera un archivo y se devuelve su ruta o contenido.
                var resultExport = new ExportReportResultDto();
                var fileBase64 = ReportExcelGenerator.GenerateExcelReport(reportDataResponse.Data.Items.ToList());
                resultExport.FileBase64 = fileBase64;
                resultExport.FileName = $"Reporte-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                resultExport.MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.Data = resultExport;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error exportando el reporte: {ex.Message}");
                response = ResponseDto.Error<ExportReportResultDto>(ex.Message);
            }
            return response;
        }
    }
}