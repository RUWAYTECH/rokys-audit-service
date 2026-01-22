using System;

namespace Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult
{
    public class ChangePeriodAuditGroupResultOrderRequestDto
    {
        public Guid PeriodAuditId { get; set; }
        public int CurrentPosition { get; set; }
        public int NewPosition { get; set; }
    }
}
