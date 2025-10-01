namespace Rokys.Audit.Model.Tables
{
    public class Group : RiskCommonEntity
    {
        public Guid GroupId { get; set; } = Guid.NewGuid();
        public Guid EnterpriseId { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Enterprise? Enterprise { get; set; }
        public virtual ICollection<ScaleGroup> ScaleGroups { get; set; } = new List<ScaleGroup>();
        public virtual ICollection<PeriodAuditResult> PeriodAuditResults { get; set; } = new List<PeriodAuditResult>();
    }
}