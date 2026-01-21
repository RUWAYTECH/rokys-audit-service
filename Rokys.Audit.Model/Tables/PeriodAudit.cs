namespace Rokys.Audit.Model.Tables
{
    public class PeriodAudit : AuditEntity
    {
        public Guid PeriodAuditId { get; set; } = Guid.NewGuid();

        // Store / audit identification
        public Guid? StoreId { get; set; }

        // Dates
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ReportDate { get; set; }

        // Additional information
        public int? AuditedDays { get; set; }
        public string GlobalObservations { get; set; } = string.Empty;
        public decimal TotalWeighting { get; set; }

        public Guid? StatusId { get; set; } // ID de Estado

        // Puntuación
        public decimal ScoreValue { get; set; }
        public string? ScaleCode { get; set; } = string.Empty;
        public string ScaleName { get; set; } = string.Empty; // Nombre de la Escala
        public string ScaleColor { get; set; } = string.Empty; // Color de la Escala
        public decimal ScaleMinValue { get; set; } // Valor Mínimo de la Escala
        public decimal ScaleMaxValue { get; set; } // Valor Máximo de la Escala

        public string? CorrelativeNumber { get; set; } // Número Correlativo

        public bool IsActive { get; set; } = true;

        public DateTime? ActionPlanCompletedDate { get; set; } // Fecha de Finalización del Plan de Acción

        // Navigation properties
        public virtual Stores? Store { get; set; }
        public virtual AuditStatus? AuditStatus { get; set; } = null!;

        public virtual ICollection<PeriodAuditResult> PeriodAuditResults { get; set; } = new List<PeriodAuditResult>();
        public virtual ICollection<PeriodAuditGroupResult> PeriodAuditGroupResults { get; set; } = new List<PeriodAuditGroupResult>();
        
        // Participantes en la auditoría
        public virtual ICollection<PeriodAuditParticipant> PeriodAuditParticipants { get; set; } = new List<PeriodAuditParticipant>();
    }
}