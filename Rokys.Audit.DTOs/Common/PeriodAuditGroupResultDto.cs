namespace Rokys.Audit.DTOs.Common
{
    public class PeriodAuditGroupResultDto
    {
        public Guid PeriodAuditId { get; set; }
        public Guid GroupId { get; set; }
        public decimal ScoreValue { get; set; }
        public string? Observations { get; set; }
        public string? ScaleDescription { get; set; }
        public decimal? TotalWeighting { get; set; }
        public string? ScaleColor { get; set; }
        public bool? HasEvidence { get; set; }
    }
}
