namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditScaleResult : AuditEntity
    {
        public Guid PeriodAuditScaleResultId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditResultId { get; set; }
        public Guid ScaleGroupId { get; set; }

        // Calculation data at the time of audit
        public decimal TotalValue { get; set; }

        // Historical weighting and thresholds
        public decimal AppliedLowRisk { get; set; }
        public decimal AppliedModerateRisk { get; set; }
        public decimal AppliedHighRisk { get; set; }
        public decimal AppliedRiskCritical { get; set; }
        public decimal AppliedWeighting { get; set; }

        public string? Observations { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAuditResult PeriodAuditResult { get; set; } = null!;
        public virtual ScaleGroup ScaleGroup { get; set; } = null!;
        public virtual ICollection<PeriodAuditTableScaleTemplateResult> PeriodAuditTableScaleTemplateResults { get; set; } = new List<PeriodAuditTableScaleTemplateResult>();
        public virtual ICollection<PeriodAuditScoringCriteriaResult> PeriodAuditScoringCriteriaResults { get; set; } = new List<PeriodAuditScoringCriteriaResult>();
        public virtual ICollection<PeriodAuditScaleSubResult> PeriodAuditScaleSubResults { get; set; } = new List<PeriodAuditScaleSubResult>();
    }
}