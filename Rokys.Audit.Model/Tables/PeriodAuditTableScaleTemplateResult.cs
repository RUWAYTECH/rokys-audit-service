namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditTableScaleTemplateResult : AuditEntity
    {
        public Guid PeriodAuditTableScaleTemplateResultId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditScaleResultId { get; set; }
        public Guid TableScaleTemplateId { get; set; }
        public string? TemplateData { get; set; } // JSON almacenado como texto
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAuditScaleResult PeriodAuditScaleResult { get; set; } = null!;
        public virtual TableScaleTemplate TableScaleTemplate { get; set; } = null!;
        public virtual ICollection<PeriodAuditFieldValues> PeriodAuditFieldValues { get; set; } = new List<PeriodAuditFieldValues>();
    }
}