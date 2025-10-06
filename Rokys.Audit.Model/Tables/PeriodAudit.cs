namespace Rokys.Audit.Model.Tables
{
    public class PeriodAudit : AuditEntity
    {
        public Guid PeriodAuditId { get; set; } = Guid.NewGuid();

        // Store / audit identification
        public Guid? StoreId { get; set; }

        // Participants
        public Guid? AdministratorId { get; set; }
        public Guid? AssistantId { get; set; }
        public Guid? OperationManagersId { get; set; }
        public Guid? FloatingAdministratorId { get; set; }
        public Guid? ResponsibleAuditorId { get; set; }

        // Dates
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ReportDate { get; set; }

        // Additional information
        public int? AuditedDays { get; set; }
        public string GlobalObservations { get; set; } = string.Empty;
        public decimal TotalWeighting { get; set; }

        public Guid StatusId { get; set; } // ID de Estado



        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Stores? Store { get; set; }
        public virtual AuditStatus AuditStatus { get; set; } = null!;
        public virtual ICollection<PeriodAuditResult> PeriodAuditResults { get; set; } = new List<PeriodAuditResult>();
    }
}