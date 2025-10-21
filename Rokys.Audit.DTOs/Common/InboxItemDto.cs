using System;

namespace Rokys.Audit.DTOs.Common
{
    public class InboxItemDto
    {
        public Guid? PeriodAuditId { get; set; }
        public Guid? PrevStatusId { get; set; }
        public Guid? NextStatusId { get; set; }
        public Guid? PrevUserId { get; set; }
        public Guid? NextUserId { get; set; }
        public Guid? ApproverId { get; set; }
    public string? Comments { get; set; }
    // DueDate and Priority were removed from the InboxItems domain per recent design decision
        public bool IsActive { get; set; }
    public string? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
    public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
