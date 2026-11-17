using System.Text.Json.Serialization;

namespace Rokys.Audit.DTOs.Requests.PeriodAudit
{
    public class PeriodAuditExportRequestDto
    {
        public Guid? EnterpriseGroupingId { get; set; }
        public Guid? EnterpriseId { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? ResponsibleAuditorId { get; set; }
        public Guid? AuditStatusId { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Filter { get; set; }
        public string? EndDate { get; set; }
        public string? StartDate { get; set; }
        public Guid[]? PeriodAuditIds { get; set; }
    }
}
