namespace Rokys.Audit.Model.Tables
{
    public class CriteriaSubResult : AuditEntity
    {
        public Guid CriteriaSubResultId { get; set; } = Guid.NewGuid();
        public Guid ScaleGroupId { get; set; }
        public Guid? AuditTemplateFieldId { get; set; }
        
        // Identificación del Criterio
        public string CriteriaName { get; set; } = string.Empty;
        public string? CriteriaCode { get; set; }
        
        // Fórmula y Evaluación
        public string? ResultFormula { get; set; }
        public string ColorCode { get; set; } = string.Empty;
        
        // Puntuación
        public decimal? Score { get; set; }
        
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ScaleGroup? ScaleGroup { get; set; }
        public virtual AuditTemplateFields? AuditTemplateField { get; set; }
    }
}