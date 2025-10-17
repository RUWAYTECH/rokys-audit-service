using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditScoringCriteriaResult
{
    public class PeriodAuditScoringCriteriaResultResponseDto : PeriodAuditScoringCriteriaResultDto
    {
        public Guid PeriodAuditScoringCriteriaResultId { get; set; }
        public bool IsActive { get; set; }
    }
}
