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
                        Name = $"{supervisorGroup.First().PeriodAuditParticipants.FirstOrDefault(p => p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code)?.UserReference?.FullName ?? "Supervisor"}",
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

                if (reportSearchFilterRequestDto.StoreId.HasValue)
                    filter = filter.AndAlso(x => x.StoreId == reportSearchFilterRequestDto.StoreId.Value && x.IsActive);

                if (reportSearchFilterRequestDto.EnterpriseId.HasValue)
                    filter = filter.AndAlso(x => x.Store.EnterpriseId == reportSearchFilterRequestDto.EnterpriseId.Value && x.IsActive);

                if (reportSearchFilterRequestDto.ResponsibleAuditorId.HasValue)
                    filter = filter.AndAlso(x => x.PeriodAuditParticipants.Any(p => p.UserReferenceId == reportSearchFilterRequestDto.ResponsibleAuditorId.Value && p.RoleCodeSnapshot == RoleCodes.Auditor.Code) && x.IsActive);

                if (reportSearchFilterRequestDto.SupervisorId.HasValue)
                    filter = filter.AndAlso(x => x.PeriodAuditParticipants.Any(p => p.UserReferenceId == reportSearchFilterRequestDto.SupervisorId.Value && p.RoleCodeSnapshot == RoleCodes.JobSupervisor.Code) && x.IsActive);

                if (reportSearchFilterRequestDto.ReportDate.HasValue)
                {
                    var date = reportSearchFilterRequestDto.ReportDate.Value;

                    var startDate = new DateTime(date.Year, date.Month, 1, 0, 0, 0);

                    var endDate = startDate.AddMonths(1).AddTicks(-1);

                    filter = filter.AndAlso(x =>
                        x.CreationDate >= startDate &&
                        x.CreationDate <= endDate &&
                        x.IsActive
                    );
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