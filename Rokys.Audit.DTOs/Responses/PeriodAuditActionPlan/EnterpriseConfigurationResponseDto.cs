using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.PeriodAuditActionPlan
{
    public class EnterpriseConfigurationResponseDto
    {
        public Guid EnterpriseId { get; set; }
        public string EnterpriseName { get; set; } = string.Empty;
        public string EnterpriseCode { get; set; } = string.Empty;
        public bool HasConfiguration { get; set; }
        public decimal ConfigurationValue { get; set; }
    }
}
