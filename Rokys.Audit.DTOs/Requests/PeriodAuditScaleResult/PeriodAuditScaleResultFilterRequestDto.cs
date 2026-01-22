using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult
{
    public class PeriodAuditScaleResultFilterRequestDto : PaginationRequestDto
    {
        public Guid? PeriodAuditGroupResultId { get; set; }
        public Guid? ScaleGroupId { get; set; }
    }
}
