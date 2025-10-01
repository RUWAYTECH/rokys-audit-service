namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditScaleResult : AuditEntity
    {
        public Guid PeriodAuditScaleResultId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditResultId { get; set; }
        public Guid ScaleGroupId { get; set; }

        // Calculation data at the time of audit
        public decimal TotalValue { get; set; }
        public string? RiskLevel { get; set; }

        // Historical weighting and thresholds
        public decimal AppliedWeighting { get; set; }
        public decimal AppliedLowThreshold { get; set; }
        public decimal AppliedModerateThreshold { get; set; }
        public decimal AppliedHighThreshold { get; set; }

        public string? Observations { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAuditResult PeriodAuditResult { get; set; } = null!;
        public virtual ScaleGroup ScaleGroup { get; set; } = null!;
        public virtual ICollection<PeriodAuditFieldValues> PeriodAuditFieldValues { get; set; } = new List<PeriodAuditFieldValues>();
    }
}