using System;

namespace Rokys.Audit.DTOs.Requests.PeriodAuditTableScaleTemplateResult
{
    public class PeriodAuditTableScaleTemplateResultFilterRequestDto
    {
        public Guid? PeriodAuditScaleResultId { get; set; }
        public Guid? TableScaleTemplateId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
