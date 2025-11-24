using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditGroupResult
{
    public class PeriodAuditGroupResultResponseDto : PeriodAuditGroupResultDto
    {
        public Guid PeriodAuditGroupResultId { get; set; }
        public bool IsActive { get; set; }
        public string? GroupName { get; set; }
        public int? SortOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
