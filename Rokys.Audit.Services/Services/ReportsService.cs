using Microsoft.Extensions.Logging;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Reports;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.Services.Services
{
    /// <summary>
    /// Servicio para generar reportes y datos de dashboard
    /// </summary>
    public class ReportsService : IReportsService
    {
        private readonly IPeriodAuditRepository _periodAuditRepository;
        private readonly ILogger<ReportsService> _logger;

        public ReportsService(
            IPeriodAuditRepository periodAuditRepository,
            ILogger<ReportsService> logger)
        {
            _periodAuditRepository = periodAuditRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<DashboardDataResponseDto>> GetDashboardEvolutionsDataAsync(int year)
        {
            var response = ResponseDto.Create<DashboardDataResponseDto>();
            try
            {
                // Obtener auditorías del año especificado
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31, 23, 59, 59);

                var periodAudits = await _periodAuditRepository.GetAsync(
                    filter: x => x.CreationDate >= startDate && x.CreationDate <= endDate && x.IsActive && x.AuditStatus.Code == "FIN");

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

                // Agregar serie de tiendas auditadas por mes
                var storesAuditedData = new List<decimal>();
                for (int month = 1; month <= 12; month++)
                {
                    var uniqueStoresCount = periodAudits
                        .Where(x => x.CreationDate.Month == month)
                        .Select(x => x.StoreId)
                        .Distinct()
                        .Count();
                    storesAuditedData.Add(uniqueStoresCount);
                }

                series.Add(new DashboardSeriesDto
                {
                    Name = "Tiendas auditadas",
                    Type = "spline",
                    YAxis = 1,
                    Color = "#ff6b35",
                    Data = storesAuditedData,
                    Tooltip = new DashboardTooltipDto { ValueSuffix = " tiendas" }
                });

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
    }
}