using System.Text.Json.Serialization;

namespace Rokys.Audit.DTOs.Requests.PeriodAudit
{
    public class PeriodAuditExportRequestDto : PeriodAuditFilterRequestDto
    {
        public new int? PageNumber { get; set; }
        public new int? PageSize { get; set; }
        public new string? EndDate { get; set; }
        public new string? StartDate { get; set; }
        public Guid[]? PeriodAuditIds { get; set; }
    }
}
