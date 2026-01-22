namespace Rokys.Audit.Model.Tables
{
    public class AuditStatus : AuditEntity
    {
        public Guid AuditStatusId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? ColorCode { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<PeriodAudit> PeriodAudits { get; set; } = new List<PeriodAudit>();

        // InboxItems where this status was the previous status
        public virtual ICollection<InboxItems> PrevInboxItems { get; set; } = new List<InboxItems>();

        // InboxItems where this status is the next status
        public virtual ICollection<InboxItems> NextInboxItems { get; set; } = new List<InboxItems>();
    }
}