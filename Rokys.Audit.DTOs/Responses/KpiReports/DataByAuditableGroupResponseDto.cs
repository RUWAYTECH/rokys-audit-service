namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataByAuditableGroupResponseDto
    {
        public string GroupId { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
    }
}
