using System;

namespace Rokys.Audit.Model.Tables
{
    public class InboxItems : AuditEntity
    {
        public Guid InboxItemId { get; set; } = Guid.NewGuid();
        public Guid? PeriodAuditId { get; set; }
        public Guid? PrevStatusId { get; set; }
        public Guid? NextStatusId { get; set; }
        public Guid? PrevUserId { get; set; }
        public Guid? NextUserId { get; set; }
        public Guid? ApproverId { get; set; }
    public string? Comments { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
