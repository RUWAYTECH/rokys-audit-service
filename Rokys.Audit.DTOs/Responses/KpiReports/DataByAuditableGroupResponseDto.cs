namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataByAuditableGroupResponseDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string RiskColor { get; set; } = string.Empty;
        public AuditablePointsResponseDto[] AuditablePoints { get; set; } = [];
    }

    public class AuditablePointsResponseDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string RiskColor { get; set; } = string.Empty;
        public MonthlyAuditablePointDataDto[] Months { get; set; } = Array.Empty<MonthlyAuditablePointDataDto>();
    }

    public class MonthlyAuditablePointDataDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string RiskColor { get; set; } = string.Empty;
    }
}
