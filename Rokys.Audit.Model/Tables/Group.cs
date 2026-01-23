namespace Rokys.Audit.Model.Tables
{
    public class Group : AuditEntity
    {
        public Guid GroupId { get; set; }
        public Guid? EnterpriseId { get; set; }
        public Guid? EnterpriseGroupingId { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Weighting { get; set; }
        public int SortOrder { get; set; } = 0;
        //public decimal? NormalizedScore { get; set; }
        //public decimal? ExpectedDistribution { get; set; }
        //public int LevelOrder { get; set; } = 1;
        //public string? ScaleType { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Enterprise? Enterprise { get; set; }
        public virtual EnterpriseGrouping? EnterpriseGrouping { get; set; }
        public virtual ICollection<ScaleGroup> ScaleGroups { get; set; } = new List<ScaleGroup>();
        public virtual ICollection<PeriodAuditResult> PeriodAuditResults { get; set; } = new List<PeriodAuditResult>();
        public virtual ICollection<PeriodAuditGroupResult> PeriodAuditGroupResults { get; set; } = new List<PeriodAuditGroupResult>();
    }
}