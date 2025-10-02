namespace Rokys.Audit.Model.Tables
{
    public class ScaleCompany : AuditEntity
    {
        public Guid ScaleCompanyId { get; set; } = Guid.NewGuid();
        public Guid EnterpriseId { get; set; }
        public string Description { get; set; } = string.Empty;

        // Objetivo general para la empresa
        public decimal ObjectiveValue { get; set; }

        // Umbrales para la empresa
        public decimal RiskLow { get; set; }
        public decimal RiskModerate { get; set; }
        public decimal RiskHigh { get; set; }

        // Riesgo crítico = mayor a RiesgoElevado
        public decimal RiskCritical { get; set; }

        // Ponderación de la empresa
        public decimal Weighting { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Enterprise Enterprise { get; set; } = null!;
    }
}
