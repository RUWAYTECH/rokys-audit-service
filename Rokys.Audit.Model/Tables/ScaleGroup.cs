namespace Rokys.Audit.Model.Tables
{
    public class ScaleGroup : RiskCommonEntity
    {
        public Guid ScaleGroupId { get; set; } = Guid.NewGuid();
        public Guid GroupId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        
        // Note: ObjectiveValue, RiskLow, RiskModerate, RiskHigh, and Weighting are inherited from RiskCommonEntity
        // But SQL uses LowRisk, ModerateRisk, HighRisk instead of RiskLow, RiskModerate, RiskHigh
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Group Group { get; set; } = null!;
        public virtual ICollection<AuditTemplateFields> AuditTemplateFields { get; set; } = new List<AuditTemplateFields>();
        public virtual ICollection<ScoringCriteria> ScoringCriteria { get; set; } = new List<ScoringCriteria>();
        public virtual ICollection<PeriodAuditScaleResult> PeriodAuditScaleResults { get; set; } = new List<PeriodAuditScaleResult>();
        public virtual ICollection<PeriodAuditFieldValues> PeriodAuditFieldValues { get; set; } = new List<PeriodAuditFieldValues>();
    }
}