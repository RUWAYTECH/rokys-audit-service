using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditScaleResult
{
    public class PeriodAuditScaleResultResponseDto : PeriodAuditScaleResultDto
    {
        public Guid PeriodAuditScaleResultId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
