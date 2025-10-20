namespace Rokys.Audit.Model.Tables
{
    public class ScaleGroup : AuditEntity
    {
        public Guid ScaleGroupId { get; set; } = Guid.NewGuid();
        public Guid GroupId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool? HasSourceData { get; set; }
        public decimal Weighting { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Group Group { get; set; } = null!;
        public virtual ICollection<TableScaleTemplate> TableScaleTemplates { get; set; } = new List<TableScaleTemplate>();
        public virtual ICollection<ScoringCriteria> ScoringCriteria { get; set; } = new List<ScoringCriteria>();
        public virtual ICollection<CriteriaSubResult> CriteriaSubResults { get; set; } = new List<CriteriaSubResult>();
        public virtual ICollection<PeriodAuditScaleResult> PeriodAuditScaleResults { get; set; } = new List<PeriodAuditScaleResult>();
    }
}