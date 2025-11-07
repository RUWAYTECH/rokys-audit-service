namespace Rokys.Audit.DTOs.Common
{
    public class PeriodAuditScaleResultDto
    {
        public Guid PeriodAuditGroupResultId { get; set; }
        public Guid ScaleGroupId { get; set; }
        public decimal? ScoreValue { get; set; }
        public decimal? AppliedWeighting { get; set; }
        public string? Observations { get; set; }
        public string? ScaleDescription { get; set; }
        public string? ScaleColor { get; set; }
        public int? SortOrder { get; set; }
    }
}
