namespace Rokys.Audit.DTOs.Requests.KpiReports
{
    public class AuditableGroupReportRequestDto
    {
        public required Guid EnterpriseGroupingId { get; set; }
        public Guid[]? EnterpriseIds { get; set; }
        public Guid[]? StoreIds { get; set; }
        public Guid[]? GroupIds { get; set; } // IDs de grupos auditables
        public string? Months { get; set; } // "01,02,03,04" etc.
    }
}
