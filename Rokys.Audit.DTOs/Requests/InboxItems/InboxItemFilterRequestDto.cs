using Rokys.Audit.DTOs.Common;
using System;

namespace Rokys.Audit.DTOs.Requests.InboxItems
{
    public class InboxItemFilterRequestDto : PaginationRequestDto
    {
        public Guid? PeriodAuditId { get; set; }
        public Guid? NextStatusId { get; set; }
    }
}
