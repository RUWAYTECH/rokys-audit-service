namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataExpirationResponseDto
    {
        public string AuditId { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public Guid? StoreId { get; set; }
        public DateTime AuditDate { get; set; }
        public int Year { get; set; }
        public string Month { get; set; } = string.Empty;
        public string SupervisorId { get; set; } = string.Empty;
        public string SupervisorName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public string? Observation { get; set; }
    }
}
