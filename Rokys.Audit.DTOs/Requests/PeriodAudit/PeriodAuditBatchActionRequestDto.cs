using System;
using System.Collections.Generic;

namespace Rokys.Audit.DTOs.Requests.PeriodAudit
{
    public class PeriodAuditBatchActionRequestDto
    {
        public List<Guid> PeriodAuditIds { get; set; } = new List<Guid>();
        // Action values: Approve, Cancel, Return
        public string Action { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }
}
