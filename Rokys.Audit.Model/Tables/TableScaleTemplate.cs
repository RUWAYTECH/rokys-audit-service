namespace Rokys.Audit.Model.Tables
{
    public class TableScaleTemplate : AuditEntity
    {
    public Guid TableScaleTemplateId { get; set; } = Guid.NewGuid();
    public Guid ScaleGroupId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? TemplateData { get; set; } // JSON almacenado como texto
    public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ScaleGroup ScaleGroup { get; set; } = null!;
        public virtual ICollection<AuditTemplateFields> AuditTemplateFields { get; set; } = new List<AuditTemplateFields>();
        public virtual ICollection<PeriodAuditTableScaleTemplateResult> PeriodAuditTableScaleTemplateResults { get; set; } = new List<PeriodAuditTableScaleTemplateResult>();
    }
}