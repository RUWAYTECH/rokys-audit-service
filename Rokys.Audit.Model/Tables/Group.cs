namespace Rokys.Audit.Model.Tables
{
    public class Group : AuditEntity
    {
        public Guid GroupId { get; set; } = Guid.NewGuid();
        public Guid EnterpriseId { get; set; }
        public string Name { get; set; } = string.Empty;

        // Objetivo general para el grupo
        public decimal ObjectiveValue { get; set; }

        // Umbrales para el grupo
        public decimal RiskLow { get; set; }
        public decimal RiskModerate { get; set; }
        public decimal RiskHigh { get; set; }

        // Riesgo crítico = mayor a RiesgoElevado
        public decimal RiskCritical { get; set; }

        // Ponderación del grupo
        public decimal Weighting { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Enterprise Enterprise { get; set; } = null!;
        public virtual ICollection<ScaleGroup> ScaleGroups { get; set; } = new List<ScaleGroup>();
        public virtual ICollection<PeriodAuditResult> PeriodAuditResults { get; set; } = new List<PeriodAuditResult>();
    }
}