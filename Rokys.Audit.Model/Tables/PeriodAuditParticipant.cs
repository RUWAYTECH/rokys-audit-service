namespace Rokys.Audit.Model.Tables
{
    public class PeriodAuditParticipant : AuditEntity
    {
        public Guid PeriodAuditParticipantId { get; set; } = Guid.NewGuid();
        public Guid PeriodAuditId { get; set; }
        public Guid UserReferenceId { get; set; }
        public string RoleCodeSnapshot { get; set; } = string.Empty;
        public string RoleNameSnapshot { get; set; } = string.Empty;
        public string? Comments { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual PeriodAudit? PeriodAudit { get; set; }
        public virtual UserReference? UserReference { get; set; }
    }
}