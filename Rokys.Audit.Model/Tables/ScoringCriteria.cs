namespace Rokys.Audit.Model.Tables
{
    public class ScoringCriteria : AuditEntity
    {
        public Guid ScoringCriteriaId { get; set; } = Guid.NewGuid();
        public Guid ScaleGroupId { get; set; }
        public Guid? AuditTemplateFieldId { get; set; }
        
        // Identificación del Criterio
        public string CriteriaName { get; set; } = string.Empty;
        public string? CriteriaCode { get; set; }
        public string? Description { get; set; }
        
        // Campo a Evaluar (referencia lógica)
        public string EvaluatedFieldCode { get; set; } = string.Empty;
        public string? EvaluatedFieldName { get; set; }
        public string? EvaluatedFieldType { get; set; }
        
        // Fórmula y Evaluación
        public string? ResultFormula { get; set; }
        public string ComparisonOperator { get; set; } = string.Empty; // '=', '!=', '>', '<', '>=', '<=', 'BETWEEN', 'IN', 'CONTAINS'
        public string ExpectedValue { get; set; } = string.Empty;
        
        // Puntuación
        public decimal Score { get; set; }
        public decimal ScoreWeight { get; set; } = 1.00m;
        
        // Configuración adicional
        public bool IsRequired { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ScaleGroup ScaleGroup { get; set; } = null!;
        public virtual AuditTemplateFields? AuditTemplateField { get; set; }
    }
}