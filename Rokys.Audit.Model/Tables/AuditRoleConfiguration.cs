namespace Rokys.Audit.Model.Tables
{
    public class AuditRoleConfiguration : AuditEntity
    {
        public Guid AuditRoleConfigurationId { get; set; } = Guid.NewGuid();
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = true;
        public bool AllowMultiple { get; set; } = false;
        public int? SequenceOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}