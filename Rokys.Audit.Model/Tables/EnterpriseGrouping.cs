namespace Rokys.Audit.Model.Tables
{
    public class EnterpriseGrouping : AuditEntity
    {
        public Guid EnterpriseGroupingId { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<EnterpriseGroup> EnterpriseGroups { get; set; } = new List<EnterpriseGroup>();
        public virtual ICollection<AuditRoleConfiguration> AuditRoleConfigurations { get; set; } = new List<AuditRoleConfiguration>();
    }
}
