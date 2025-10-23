namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditScaleResult : AuditEntity
    {
        public Guid PeriodAuditScaleResultId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditGroupResultId { get; set; }
        public Guid ScaleGroupId { get; set; }

        // Calculation data at the time of audit
        public decimal ScoreValue { get; set; }
        public int SortOrder { get; set; }

        // Historical weighting and thresholds
        public decimal AppliedWeighting { get; set; }

        public string? Observations { get; set; }
        public string? ScaleDescription { get; set; }
        public string? ScaleColor { get; set; }
        public int? SortOrder { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAuditGroupResult PeriodAuditGroupResult { get; set; } = null!;
        public virtual ScaleGroup ScaleGroup { get; set; } = null!;
        public virtual ICollection<PeriodAuditTableScaleTemplateResult> PeriodAuditTableScaleTemplateResults { get; set; } = new List<PeriodAuditTableScaleTemplateResult>();
        public virtual ICollection<PeriodAuditScoringCriteriaResult> PeriodAuditScoringCriteriaResults { get; set; } = new List<PeriodAuditScoringCriteriaResult>();
        public virtual ICollection<PeriodAuditScaleSubResult> PeriodAuditScaleSubResults { get; set; } = new List<PeriodAuditScaleSubResult>();
    }
}