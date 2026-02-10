namespace Rokys.Audit.DTOs.Requests.KpiReports
{
    public class DataByAuditableGroupRequestDto
    {
        public required Guid EnterpriseGroupingId { get; set; }
        public Guid[]? EnterpriseIds { get; set; }
        public Guid[]? StoreIds { get; set; }
        public Guid[]? UnitManagerIds { get; set; }
        public Guid[]? SupervisorIds { get; set; }
        public Guid[]? AuditorIds { get; set; }
        public Guid[]? GroupIds { get; set; } // IDs de grupos auditables
        public string? Months { get; set; } // "01,02,03,04" etc.
    }
}
