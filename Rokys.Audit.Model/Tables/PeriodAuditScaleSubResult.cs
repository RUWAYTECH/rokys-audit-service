namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditScaleSubResult : AuditEntity
    {
        public Guid PeriodAuditScaleSubResultId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditScaleResultId { get; set; }
        public Guid CriteriaSubResultId { get; set; }
        public Guid ScaleGroupId { get; set; }
        public Guid? AuditTemplateFieldId { get; set; }

        // Identificación del Criterio (desnormalizado)
        public string? CriteriaCode { get; set; }
        public string CriteriaName { get; set; } = string.Empty;

        // Valores evaluados
        public string? EvaluatedValue { get; set; }
        public string? CalculatedResult { get; set; }
        public string? AppliedFormula { get; set; }

        // Resultado de la evaluación
        public decimal? ScoreObtained { get; set; }
        public string? ColorCode { get; set; }

        // Detalles
        public string? EvaluationNotes { get; set; }
        public string? ResultMessage { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAuditScaleResult? PeriodAuditScaleResult { get; set; }
        public virtual CriteriaSubResult? CriteriaSubResult { get; set; }
        public virtual ScaleGroup? ScaleGroup { get; set; }
        public virtual AuditTemplateFields? AuditTemplateField { get; set; }
    }
}