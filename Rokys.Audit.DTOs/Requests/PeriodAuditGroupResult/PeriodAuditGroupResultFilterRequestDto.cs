using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult
{
    public class PeriodAuditGroupResultFilterRequestDto : PaginationRequestDto
    {
        public Guid? PeriodAuditId { get; set; }
        public Guid? GroupId { get; set; }
    }
}
