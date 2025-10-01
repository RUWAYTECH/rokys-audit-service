namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditFieldValues : AuditEntity
    {
        public Guid PeriodAuditFieldValueId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditId { get; set; }
        public Guid? AuditTemplateFieldId { get; set; }
        public Guid ScaleGroupId { get; set; }
        public Guid? PeriodAuditScaleResultId { get; set; }
        
        // Información del Grupo (desnormalizado para performance)
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string? Orientation { get; set; } // horizontal, vertical
        
        // Información del Campo (desnormalizado)
        public string FieldCode { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        
        // VALORES CAPTURADOS
        public string? TextValue { get; set; }
        public decimal? NumericValue { get; set; }
        public DateTime? DateValue { get; set; }
        public bool? BooleanValue { get; set; }
        public string? ImageUrl { get; set; } // Para almacenar URL o path de imagen
        
        // Metadatos del valor capturado
        public bool IsRequired { get; set; } = false;
        public string? ValidationStatus { get; set; } // 'valid', 'invalid', 'pending', 'warning'
        public string? ValidationMessage { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAudit PeriodAudit { get; set; } = null!;
        public virtual AuditTemplateFields? AuditTemplateField { get; set; }
        public virtual ScaleGroup ScaleGroup { get; set; } = null!;
        public virtual PeriodAuditScaleResult? PeriodAuditScaleResult { get; set; }
    }
}