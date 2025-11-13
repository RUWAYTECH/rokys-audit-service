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
        public int? Ranking { get; set; }
        public decimal MothlyScore { get; set; }
        public int AuditedQuantityPerStore { get; set; }
        public string LevelRisk { get; set; } = string.Empty;
        public string RiskColor { get; set; } = string.Empty;
        public AuditStatusResponseDto? AuditStatus { get; set; }
        // fin Audit Info

        //TODO review these properties
        public List<UserInfoAuditItem> Auditor { get; set; }
        public List<UserInfoAuditItem> Supervisor { get; set; }
        public List<UserInfoAuditItem> OperationManager { get; set; }
    }
    public class UserInfoAuditItem
    {
        public string FullName { get; set; } = string.Empty;
        public int TotalAudits { get; set; }
    }
}