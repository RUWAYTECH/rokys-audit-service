namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditScoringCriteriaResult : AuditEntity
    {
        public Guid PeriodAuditScoringCriteriaResultId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditScaleResultId { get; set; }

        // Identificación del Criterio
        public string CriteriaName { get; set; } = string.Empty; // Nombre del Criterio
        public string? CriteriaCode { get; set; } // Código único del criterio (opcional)

        // Fórmula y Evaluación
        public string? ResultFormula { get; set; } // Fórmula para calcular resultado del campo
        public string ComparisonOperator { get; set; } = string.Empty; // Operador: '=', '!=', '>', '<', '>=', '<=', 'BETWEEN', 'IN', 'CONTAINS'
        public string ExpectedValue { get; set; } = string.Empty; // Valor esperado

        // Puntuación
        public decimal Score { get; set; } // Puntaje otorgado si cumple
        public int SortOrder { get; set; } = 0; // Orden de evaluación

        public string? ErrorMessage { get; set; } // Mensaje si no cumple
        public string? SuccessMessage { get; set; } // Mensaje si cumple

        public decimal? ResultObtained { get; set; } // Puntaje obtenido

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAuditScaleResult PeriodAuditScaleResult { get; set; } = null!;
    }
}