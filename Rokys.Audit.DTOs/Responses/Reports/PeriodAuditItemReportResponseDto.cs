using Rokys.Audit.DTOs.Responses.AuditStatus;

namespace Rokys.Audit.DTOs.Responses.Reports
{
    public class PeriodAuditItemReportResponseDto
    {
        // Audit Info
        public Guid? StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string EnterpriseName { get; set; } = string.Empty;
        public Guid? EnterpriseId { get; set; }
        public string? AdministratorName { get; set; }
        public string? AssistantName { get; set; }
        public string? OperationManagerName { get; set; }
        public string? FloatingAdministratorName { get; set; }
        public string? ResponsibleAuditorName { get; set; }
        public string? SupervisorName { get; set; }
        public int? Ranking { get; set; }
        public decimal MothlyScore { get; set; }
        public int AuditedQuantityPerStore { get; set; }
        public string LevelRisk { get; set; }
        public string RiskColor { get; set; }
        public AuditStatusResponseDto? AuditStatus { get; set; }
        // fin Audit Info
    }
}