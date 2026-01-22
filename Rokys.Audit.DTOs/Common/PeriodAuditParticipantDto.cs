using System;

namespace Rokys.Audit.DTOs.Common
{
    public class PeriodAuditParticipantDto
    {
        public Guid UserReferenceId { get; set; }
        public string RoleCodeSnapshot { get; set; } = string.Empty;
        public string RoleNameSnapshot { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }
}