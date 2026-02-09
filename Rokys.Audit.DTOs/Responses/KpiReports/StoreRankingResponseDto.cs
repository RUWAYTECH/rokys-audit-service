namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class StoreRankingResponseDto
    {
        public int Ranking { get; set; }
        public string StoreId { get; set; } = string.Empty;
        public string Store { get; set; } = string.Empty;
        public string StoreCode { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
    }
}
