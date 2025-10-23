using Rokys.Audit.DTOs.Responses.PeriodAuditFieldValues;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditTableScaleTemplateResult
{
    public class PeriodAuditTableScaleTemplateResultListResponseDto
    {
        public Guid PeriodAuditTableScaleTemplateResultId { get; set; }
        public Guid PeriodAuditScaleResultId { get; set; }
        public Guid TableScaleTemplateId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Orientation { get; set; }
        public int? SortOrder { get; set; }
        public List<PeriodAuditFieldValuesListResponseDto> PeriodAuditFieldValues { get; set; } = new List<PeriodAuditFieldValuesListResponseDto>();
    }
}
