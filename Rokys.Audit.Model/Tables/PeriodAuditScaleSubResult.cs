namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditScaleSubResult : AuditEntity
    {
        public Guid PeriodAuditScaleSubResultId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditScaleResultId { get; set; }
        public Guid CriteriaSubResultId { get; set; }
        
        // Identificación del Criterio (desnormalizado)
        public string? CriteriaCode { get; set; }
        public string CriteriaName { get; set; } = string.Empty;
        
        // Valores evaluados
        public string? EvaluatedValue { get; set; } // Valor que se evaluó
        public string? CalculatedResult { get; set; } // Resultado de aplicar la fórmula
        public string? AppliedFormula { get; set; } // Fórmula que se aplicó (histórico)
        
        // Resultado de la evaluación
        public decimal? ScoreObtained { get; set; } // Puntaje obtenido
        public string? ColorCode { get; set; } // Código de color del resultado

        // Configuración
        public bool ForSummary { get; set; } = false; // Indica si el resultado se incluye en el resumen

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAuditScaleResult PeriodAuditScaleResult { get; set; } = null!;
        public virtual CriteriaSubResult CriteriaSubResult { get; set; } = null!;
    }
}