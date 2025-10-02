namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditResult : AuditEntity
    {
        public Guid PeriodAuditResultId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditId { get; set; }
        public Guid GroupId { get; set; }

        // Calculation data at the time of audit
        public decimal ObtainedValue { get; set; }

        // Historical weighting and thresholds
        public decimal AppliedRiskLow { get; set; }
        public decimal AppliedRiskModerate { get; set; }
        public decimal AppliedRiskHigh { get; set; }
        public decimal AppliedRiskCritical { get; set; }
        public decimal AppliedWeighting { get; set; }

        public string? Observations { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAudit PeriodAudit { get; set; } = null!;
        public virtual Group Group { get; set; } = null!;
        public virtual ICollection<PeriodAuditScaleResult> PeriodAuditScaleResults { get; set; } = new List<PeriodAuditScaleResult>();
        public virtual ICollection<EvidenceFiles> EvidenceFiles { get; set; } = new List<EvidenceFiles>();
    }
}