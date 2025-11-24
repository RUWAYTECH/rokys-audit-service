using System;

namespace Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult
{
    public class ChangePeriodAuditScaleResultOrderRequestDto
    {
        public Guid PeriodAuditGroupResultId { get; set; }
        public int CurrentPosition { get; set; }
        public int NewPosition { get; set; }
    }
}
