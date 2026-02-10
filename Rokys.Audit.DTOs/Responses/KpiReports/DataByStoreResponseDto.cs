namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataByStoreResponseDto
    {
        public string StoreId { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
    }
}
