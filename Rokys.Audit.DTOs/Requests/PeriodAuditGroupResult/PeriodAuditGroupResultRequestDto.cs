using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult
{
    public class PeriodAuditGroupResultRequestDto : PeriodAuditGroupResultDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
