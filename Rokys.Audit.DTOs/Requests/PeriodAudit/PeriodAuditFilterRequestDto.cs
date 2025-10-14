using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.PeriodAudit
{
    public class PeriodAuditFilterRequestDto : PaginationRequestDto
    {
        public Guid? EnterpriseId { get; set; }
        public Guid? StoreId { get; set; }
    }
}
