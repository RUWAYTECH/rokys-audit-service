using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.PeriodAudit
{
    public class PeriodAuditFilterRequestDto : PaginationRequestDto
    {
        public Guid? EnterpriseId { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? ResponsibleAuditorId { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public Guid? AuditStatusId { get; set; }
    }
}
