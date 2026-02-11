namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataBySupervisorStoreResponseDto
    {
        public string SupervisorId { get; set; } = string.Empty;
        public string SupervisorName { get; set; } = string.Empty;
        public string StoreId { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string StoreCode { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
    }
}
