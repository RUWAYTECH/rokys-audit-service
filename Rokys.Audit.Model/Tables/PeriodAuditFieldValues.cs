namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditFieldValues : AuditEntity
    {
        public Guid PeriodAuditFieldValueId { get; set; } = Guid.NewGuid();
        public Guid? AuditTemplateFieldId { get; set; }
        public Guid PeriodAuditTableScaleTemplateResultId { get; set; }

        // Información del Campo (desnormalizado)
        public string FieldCode { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty; // numeric, text, date, boolean, select, image
        public string IsCalculated { get; set; } // Si es un campo calculado
        public string? CalculationFormula { get; set; } // Fórmula para calcular el valor (si es calculado)
        public string? AcumulationType { get; set; } // Tipo de Acumulación: 'NA', 'SUM', 'AVERAGE', 'MAX', 'MIN', 'COUNT'
        public string? FieldOptions { get; set; } // Opciones para campos tipo 'select' (JSON)
        
        // VALORES CAPTURADOS
        public string? TextValue { get; set; }
        public decimal? NumericValue { get; set; }
        public string? DefaultValue { get; set; }
        public DateTime? DateValue { get; set; }
        public bool? BooleanValue { get; set; }
        public string? ImageUrl { get; set; } // Para almacenar URL o path de imagen
        public string? FieldOptionsValue { get; set; } // Valor seleccionado para campos tipo 'select'
        
        public string? ValidationStatus { get; set; } // 'valid', 'invalid', 'pending', 'warning'
        public string? ValidationMessage { get; set; }
        public int? SortOrder { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual AuditTemplateFields? AuditTemplateField { get; set; }
        public virtual PeriodAuditTableScaleTemplateResult? PeriodAuditTableScaleTemplateResult { get; set; }
    }
}