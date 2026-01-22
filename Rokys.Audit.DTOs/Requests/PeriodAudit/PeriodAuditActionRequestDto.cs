using System;

namespace Rokys.Audit.DTOs.Requests.PeriodAudit
{
    public class PeriodAuditActionRequestDto
    {
        // Action values: Approve, Cancel, Return
        public string Action { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }
}
