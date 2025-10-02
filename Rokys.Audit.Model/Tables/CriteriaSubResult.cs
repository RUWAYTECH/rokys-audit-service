namespace Rokys.Audit.Model.Tables
{
    public class CriteriaSubResult : AuditEntity
    {
        public Guid CriteriaSubResultId { get; set; } = Guid.NewGuid();
        public Guid ScaleGroupId { get; set; }
        
        // Identificación del Criterio
        public string CriteriaName { get; set; } = string.Empty; // Nombre del Criterio
        public string? CriteriaCode { get; set; } // Código único del criterio (opcional)
        
        // Fórmula y Evaluación
        public string? ResultFormula { get; set; } // Fórmula para calcular resultado del campo
        public string ColorCode { get; set; } = string.Empty; // Código de color para la evaluación
        
        // Puntuación
        public decimal? Score { get; set; } // Puntaje otorgado si cumple
        
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ScaleGroup ScaleGroup { get; set; } = null!;
        public virtual ICollection<PeriodAuditScaleSubResult> PeriodAuditScaleSubResults { get; set; } = new List<PeriodAuditScaleSubResult>();
    }
}