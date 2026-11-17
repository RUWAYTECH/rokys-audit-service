namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataByScaleResponseDto
    {
        public string ScaleName { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
        public MonthlyScaleDataDto[] Months { get; set; } = Array.Empty<MonthlyScaleDataDto>();
    }

    public class MonthlyScaleDataDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
    }
}
