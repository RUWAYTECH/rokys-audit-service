namespace Rokys.Audit.DTOs.Common
{
    public class ScaleCompanyDto
    {
        public Guid EnterpriseId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal ObjectiveValue { get; set; }
        public decimal RiskLow { get; set; }
        public decimal RiskModerate { get; set; }
        public decimal RiskHigh { get; set; }
        public decimal Weighting { get; set; }
    }
}
