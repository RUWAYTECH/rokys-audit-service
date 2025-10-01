namespace Rokys.Audit.Model.Tables
{
    public class ScaleCompany : AuditEntity
    {
        public Guid ScaleCompanyId { get; set; } = Guid.NewGuid();
        public string EnterpriseId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal ObjectiveValue { get; set; }
        public decimal RiskLow { get; set; }
        public decimal RiskModerate { get; set; }
        public decimal RiskHigh { get; set; }
        public decimal Weighting { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
