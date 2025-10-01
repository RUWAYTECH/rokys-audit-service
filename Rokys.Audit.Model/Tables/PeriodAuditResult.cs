namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditResult : AuditEntity
    {
        public Guid PeriodAuditResultId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditId { get; set; }
        public Guid GroupId { get; set; }

        // Calculation data at the time of audit
        public decimal ObtainedValue { get; set; }
        public string? RiskLevel { get; set; }

        // Historical weighting and thresholds
        public decimal AppliedWeighting { get; set; }
        public decimal AppliedLowThreshold { get; set; }
        public decimal AppliedModerateThreshold { get; set; }
        public decimal AppliedHighThreshold { get; set; }

        public string? Observations { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAudit PeriodAudit { get; set; } = null!;
        public virtual Group Group { get; set; } = null!;
        public virtual ICollection<PeriodAuditScaleResult> PeriodAuditScaleResults { get; set; } = new List<PeriodAuditScaleResult>();
    }
}