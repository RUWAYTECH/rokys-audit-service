namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataByStoreResponseDto
    {
        public string StoreId { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
        public string RiskColor { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = string.Empty;
        public MonthlyStoreDataDto[] Months { get; set; } = Array.Empty<MonthlyStoreDataDto>();
    }

    public class MonthlyStoreDataDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string RiskColor { get; set; } = string.Empty;
    }
}
