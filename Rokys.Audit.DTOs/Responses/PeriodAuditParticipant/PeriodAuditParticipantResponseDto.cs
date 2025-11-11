using System;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditParticipant
{
    public class PeriodAuditParticipantResponseDto
    {
        public Guid PeriodAuditParticipantId { get; set; }
        public Guid PeriodAuditId { get; set; }
        public Guid UserReferenceId { get; set; }
        public string RoleCodeSnapshot { get; set; } = string.Empty;
        public string RoleNameSnapshot { get; set; } = string.Empty;
        public string? Comments { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        
        // Navigation properties for display
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
    }
}