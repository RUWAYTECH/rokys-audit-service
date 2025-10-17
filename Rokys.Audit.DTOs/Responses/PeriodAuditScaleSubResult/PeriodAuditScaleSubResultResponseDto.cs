using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditScaleSubResult
{
    public class PeriodAuditScaleSubResultResponseDto : PeriodAuditScaleSubResultDto
    {
        public Guid PeriodAuditScaleSubResultId { get; set; }
        public bool IsActive { get; set; }
    }
}
