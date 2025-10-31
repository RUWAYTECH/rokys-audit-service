using System;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.AuditStatus;

namespace Rokys.Audit.DTOs.Responses.PeriodAudit
{
    public class PeriodAuditResponseDto : PeriodAuditDto
    {
        public Guid PeriodAuditId { get; set; }

        public decimal ScoreValue { get; set; }
        public string ScaleName { get; set; } = string.Empty;
        public string ScaleIcon { get; set; } = string.Empty;
        public string ScaleColor { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string EnterpriseName { get; set; } = string.Empty;
        public Guid? EnterpriseId { get; set; }
        public string? AdministratorName { get; set; }
        public string? AssistantName { get; set; }
        public string? OperationManagerName { get; set; }
        public string? FloatingAdministratorName { get; set; }
        public string? ResponsibleAuditorName { get; set; }
        public string? SupervisorName { get; set; }
        public string? StatusName { get; set; }
        public decimal ScaleMinValue { get; set; }
        public decimal ScaleMaxValue { get; set; }
        public decimal TotalWeighting { get; set; }
        public string? GlobalObservations { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public string? CorrelativeNumber { get; set; }
        public bool? IAmAuditor { get; set; } = false;
        public AuditStatusResponseDto? AuditStatus { get; set; }
        // Nested inbox items related to this period audit
        public List<Rokys.Audit.DTOs.Responses.InboxItems.InboxItemResponseDto>? InboxItems { get; set; }
    }
}