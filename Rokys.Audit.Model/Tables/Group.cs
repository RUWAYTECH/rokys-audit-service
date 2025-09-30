namespace Rokys.Audit.Model.Tables
{
    public class Group : RiskCommonEntity
    {
        public Guid GroupId { get; set; } = Guid.NewGuid();
        public Guid EnterpriseId { get; set; }
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<ScaleGroup> ScaleGroups { get; set; } = new List<ScaleGroup>();
    }
}