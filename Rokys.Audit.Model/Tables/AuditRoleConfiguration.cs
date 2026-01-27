namespace Rokys.Audit.Model.Tables
{
    public class AuditRoleConfiguration : AuditEntity
    {
        public Guid AuditRoleConfigurationId { get; set; } = Guid.NewGuid();
        public Guid? EnterpriseId { get; set; }
        public Guid EnterpriseGroupingId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = true;
        public bool AllowMultiple { get; set; } = false;
        public int? SequenceOrder { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Enterprise? Enterprise { get; set; }
        public virtual EnterpriseGrouping EnterpriseGrouping { get; set; }
    }
}