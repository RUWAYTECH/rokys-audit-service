namespace Rokys.Audit.Model.Tables
{
    public class AuditTemplateFields : AuditEntity
    {
        public Guid AuditTemplateFieldId { get; set; } = Guid.NewGuid();
        public Guid TableScaleTemplateId { get; set; }

        // Información del Campo
        public string FieldCode { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty; // numeric, text, date, boolean, select, image
        public string? IsCalculated { get; set; } // Si es un campo calculado
        public string? CalculationFormula { get; set; } // Fórmula para calcular el valor (si es calculado)
        public string? AcumulationType { get; set; } // Tipo de Acumulación: 'NA', 'SUM', 'AVERAGE', 'MAX', 'MIN', 'COUNT'
        public string? FieldOptions { get; set; } // Opciones para campos tipo 'select' (JSON)

        // Metadatos
    public int SortOrder { get; set; } = 0;
        public string? DefaultValue { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual TableScaleTemplate TableScaleTemplate { get; set; } = null!;
        public virtual ICollection<PeriodAuditFieldValues> PeriodAuditFieldValues { get; set; } = new List<PeriodAuditFieldValues>();
    }
}