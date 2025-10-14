namespace Rokys.Audit.Model.Tables
{
    public class Group : AuditEntity
    {
        public Guid GroupId { get; set; }
        public Guid EnterpriseId { get; set; }
    public string Name { get; set; } = string.Empty;
        public decimal Weighting { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
    public virtual Enterprise? Enterprise { get; set; }
    public virtual ICollection<ScaleGroup> ScaleGroups { get; set; } = new List<ScaleGroup>();
    public virtual ICollection<PeriodAuditResult> PeriodAuditResults { get; set; } = new List<PeriodAuditResult>();
    public virtual ICollection<PeriodAuditGroupResult> PeriodAuditGroupResults { get; set; } = new List<PeriodAuditGroupResult>();
    }
}