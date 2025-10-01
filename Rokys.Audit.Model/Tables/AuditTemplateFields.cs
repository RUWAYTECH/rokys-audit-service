namespace Rokys.Audit.Model.Tables
{
    public class AuditTemplateFields : AuditEntity
    {
        public Guid AuditTemplateFieldId { get; set; } = Guid.NewGuid();
        public Guid ScaleGroupId { get; set; }
        public Guid AuditScaleTemplateId { get; set; }
        
        // Información del Grupo
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string Orientation { get; set; } = string.Empty; // VARCHAR(2) NOT NULL
        
        // Información del Campo
        public string FieldCode { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty; // numeric, text, date, boolean, select, image
        
        // Metadatos
        public string? DefaultValue { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ScaleGroup ScaleGroup { get; set; } = null!;
        public virtual AuditScaleTemplate AuditScaleTemplate { get; set; } = null!;
        public virtual ICollection<ScoringCriteria> ScoringCriteria { get; set; } = new List<ScoringCriteria>();
        public virtual ICollection<PeriodAuditFieldValues> PeriodAuditFieldValues { get; set; } = new List<PeriodAuditFieldValues>();
    }
}