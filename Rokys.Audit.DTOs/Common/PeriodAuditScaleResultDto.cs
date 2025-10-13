namespace Rokys.Audit.DTOs.Common
{
    public class PeriodAuditScaleResultDto
    {
        public Guid PeriodAuditResultId { get; set; }
        public Guid ScaleGroupId { get; set; }
        public decimal ScoreValue { get; set; }
        public decimal AppliedWeighting { get; set; }
        public string? Observations { get; set; }
        public bool IsActive { get; set; }
    }
}
