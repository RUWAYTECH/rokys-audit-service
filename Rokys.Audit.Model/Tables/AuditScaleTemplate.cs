namespace Rokys.Audit.Model.Tables
{
    public class AuditScaleTemplate : AuditEntity
    {
        public Guid AuditScaleTemplateId { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TemplateData { get; set; } = string.Empty; // JSON almacenado como texto
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<AuditTemplateFields> AuditTemplateFields { get; set; } = new List<AuditTemplateFields>();
    }
}