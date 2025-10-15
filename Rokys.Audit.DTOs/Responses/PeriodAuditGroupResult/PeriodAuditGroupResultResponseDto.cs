using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditGroupResult
{
    public class PeriodAuditGroupResultResponseDto : PeriodAuditGroupResultDto
    {
        public Guid PeriodAuditGroupResultId { get; set; }
        public bool IsActive { get; set; }
        public string? GroupName { get; set; }
    }
}
