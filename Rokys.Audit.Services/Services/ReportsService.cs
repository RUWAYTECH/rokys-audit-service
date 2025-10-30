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

namespace Rokys.Audit.Services.Services
{
    /// <summary>
    /// Servicio para generar reportes y datos de dashboard
    /// </summary>
    public class ReportsService : IReportsService
    {
        private readonly IPeriodAuditRepository _periodAuditRepository;
        private readonly ILogger<ReportsService> _logger;

        private readonly IAMapper _mapper;

        public ReportsService(
            IPeriodAuditRepository periodAuditRepository,
            ILogger<ReportsService> logger,
            IAMapper mapper)
        {
            _periodAuditRepository = periodAuditRepository;
            _logger = logger;
            _mapper = mapper;
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
                    .GroupBy(x => new { x.StoreId, x.ScaleCode, x.ScaleName, x.ScaleColor })
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
                // Obtener auditorías del año especificado y filtradas por supervisores
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31, 23, 59, 59);
                Expression<Func<PeriodAudit, bool>> filter = x => x.CreationDate >= startDate &&
                                                                                             x.CreationDate <= endDate &&
                                                                                             x.IsActive &&
                                                                                             x.Store.EnterpriseId == enterpriseId &&
                                                                                             x.AuditStatus != null &&
                                                                                             x.AuditStatus.Code == AuditStatusCode.Completed;

                if (supervisorIds != null && supervisorIds.Length > 0)
                {
                    filter = filter.AndAlso(x => supervisorIds.Contains(x.SupervisorId ?? Guid.Empty));
                }

                var periodAudits = await _periodAuditRepository.GetAsync(
                    filter: filter,
                    includeProperties: [a => a.Supervisor!]);

                // Crear las categorías (meses)
                var categories = new List<string>
                {
                    "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                };

                // Agrupar auditorías por supervisor
                var auditsBySupervisor = periodAudits
                    .Where(x => x.Supervisor != null)
                    .GroupBy(x => x.SupervisorId!)
                    .ToList();

                // Crear series dinámicamente por supervisor
                var series = new List<DashboardSeriesDto>();
                var dashStyles = new[] { "ShortDot", "Dash", "ShortDash", "LongDash", "DashDot" };

                var supervisorIndex = 0;
                foreach (var supervisorGroup in auditsBySupervisor)
                {
                    var supervisor = supervisorGroup.Key;

                    // Calcular datos por mes para este supervisor (contar auditorías)
                    var monthlyData = new List<decimal>();
                    for (int month = 1; month <= 12; month++)
                    {
                        // Contar auditorías por mes
                        var countForMonth = supervisorGroup.Count(x => x.CreationDate.Month == month);
                        monthlyData.Add(countForMonth);
                    }

                    // Generar color único para cada supervisor
                    var color = supervisorIndex < Constants.ColorPalette.Palette.Length
                        ? Constants.ColorPalette.Palette[supervisorIndex]
                        : GenerateColor.GenerateColorFromIndex(supervisorIndex);

                    // Crear serie para este supervisor
                    var seriesDto = new DashboardSeriesDto
                    {
                        Name = $"{supervisorGroup.First().Supervisor!.FirstName} {supervisorGroup.First().Supervisor!.LastName}",
                        Type = "spline",
                        Data = monthlyData,
                        Color = color,
                        DashStyle = supervisorIndex == 0 ? null : dashStyles[(supervisorIndex - 1) % dashStyles.Length]
                    };

                    series.Add(seriesDto);
                    supervisorIndex++;
                }

                var dashboardData = new DashboardDataResponseDto
                {
                    Categories = categories,
                    Series = series
                };

                response.Data = dashboardData;

                _logger.LogInformation($"Dashboard supervisors data generated successfully for year {year} with {supervisorIds.Length} supervisors");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating dashboard supervisors data for year {year}: {ex.Message}");
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
                if (!string.IsNullOrEmpty(reportSearchFilterRequestDto.Filter))
                    filter = filter.AndAlso(x => x.CorrelativeNumber.Contains(reportSearchFilterRequestDto.Filter) && x.IsActive);

                if (reportSearchFilterRequestDto.StoreId.HasValue)
                    filter = filter.AndAlso(x => x.StoreId == reportSearchFilterRequestDto.StoreId.Value && x.IsActive);

                if (reportSearchFilterRequestDto.EnterpriseId.HasValue)
                    filter = filter.AndAlso(x => x.Store.EnterpriseId == reportSearchFilterRequestDto.EnterpriseId.Value && x.IsActive);

                if (reportSearchFilterRequestDto.ResponsibleAuditorId.HasValue)
                    filter = filter.AndAlso(x => x.ResponsibleAuditorId == reportSearchFilterRequestDto.ResponsibleAuditorId.Value && x.IsActive);

                if (reportSearchFilterRequestDto.ReportDate.HasValue)
                    filter = filter.AndAlso(x => x.CreationDate.Date == reportSearchFilterRequestDto.ReportDate.Value.Date && x.IsActive);

                Func<IQueryable<PeriodAudit>, IOrderedQueryable<PeriodAudit>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _periodAuditRepository.GetAsync(
                    filter: filter,
                    orderBy: orderBy,
                    includeProperties: [x => x.Store.Enterprise, x => x.Administrator, x => x.Assistant, x => x.OperationManager, x => x.FloatingAdministrator, x => x.ResponsibleAuditor, x => x.AuditStatus, su => su.Supervisor]
                );

                var dataResult = new PeriodAuditReportResponseDto
                {
                    Items = _mapper.Map<List<PeriodAuditItemReportResponseDto>>(entities),

                    Summaries = new List<SummaryReportResponseDto>{ new SummaryReportResponseDto
                    {
                        Ranking = entities.Count,
                        ResultByMonth = "",
                        Risk = "",
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
    }
}