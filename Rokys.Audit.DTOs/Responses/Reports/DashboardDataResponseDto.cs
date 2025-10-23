namespace Rokys.Audit.DTOs.Responses.Reports
{
    /// <summary>
    /// DTO de respuesta para datos del dashboard
    /// </summary>
    public class DashboardDataResponseDto
    {
        /// <summary>
        /// Categorías (meses del año)
        /// </summary>
        public List<string> Categories { get; set; } = new List<string>();

        /// <summary>
        /// Series de datos para el gráfico
        /// </summary>
        public List<DashboardSeriesDto> Series { get; set; } = new List<DashboardSeriesDto>();
    }

    /// <summary>
    /// Serie de datos para el dashboard
    /// </summary>
    public class DashboardSeriesDto
    {
        /// <summary>
        /// Nombre de la serie
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de gráfico (column, spline, etc.)
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Datos de la serie por mes
        /// </summary>
        public List<decimal> Data { get; set; } = new List<decimal>();

        /// <summary>
        /// Color de la serie
        /// </summary>
        public string Color { get; set; } = string.Empty;

        /// <summary>
        /// Estilo de línea (Solid, ShortDot, Dash, ShortDash, etc.)
        /// </summary>
        public string? DashStyle { get; set; }

        /// <summary>
        /// Eje Y para la serie (opcional, para múltiples ejes)
        /// </summary>
        public int? YAxis { get; set; }

        /// <summary>
        /// Configuración del tooltip
        /// </summary>
        public DashboardTooltipDto? Tooltip { get; set; }
    }

    /// <summary>
    /// Configuración del tooltip para series
    /// </summary>
    public class DashboardTooltipDto
    {
        /// <summary>
        /// Sufijo del valor en el tooltip
        /// </summary>
        public string? ValueSuffix { get; set; }
    }
}