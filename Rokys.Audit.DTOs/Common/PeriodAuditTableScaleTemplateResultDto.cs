using System;

namespace Rokys.Audit.DTOs.Common
{
    public class PeriodAuditTableScaleTemplateResultDto
    {
        public Guid PeriodAuditScaleResultId { get; set; }
        public Guid? TableScaleTemplateId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Orientation { get; set; }
        public string? TemplateData { get; set; }
        public int? SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
