using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rokys.Audit.DTOs.Responses.AuditStatus;

namespace Rokys.Audit.DTOs.Responses.Reports
{
    public class PeriodAuditItemReportResponseDto
    {
        public string ScaleCode { get; set; }
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
        public AuditStatusResponseDto? AuditStatus { get; set; }
    }
}