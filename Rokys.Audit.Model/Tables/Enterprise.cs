namespace Rokys.Audit.Model.Tables
{
    public class Enterprise : AuditEntity
    {
        public Guid EnterpriseId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Stores> Stores { get; set; } = new List<Stores>();
        public virtual ICollection<ScaleCompany> ScaleCompanies { get; set; } = new List<ScaleCompany>();
        public virtual ICollection<AuditRoleConfiguration> AuditRoleConfigurations { get; set; } = new List<AuditRoleConfiguration>();
        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
        public EnterpriseTheme? Theme { get; set; }
    }
}