using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditTableScaleTemplateResult
{
    public class PeriodAuditTableScaleTemplateResultResponseDto : PeriodAuditTableScaleTemplateResultDto
    {
        public Guid PeriodAuditTableScaleTemplateResultId { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
