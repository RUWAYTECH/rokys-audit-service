namespace Rokys.Audit.DTOs.Common
{
    public class GroupDto
    {
        public Guid EnterpriseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal? ObjectiveValue { get; set; }
        public decimal? RiskLow { get; set; }
        public decimal? RiskModerate { get; set; }
        public decimal? RiskHigh { get; set; }
        public decimal? RiskCritical { get; set; }
        public decimal? GroupWeight { get; set; }
    }
}