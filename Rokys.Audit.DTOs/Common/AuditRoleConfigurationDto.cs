namespace Rokys.Audit.DTOs.Common
{
    public class AuditRoleConfigurationDto
    {
        public Guid? EnterpriseId { get; set; }        public Guid EnterpriseGroupingId { get; set; }        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = true;
        public bool AllowMultiple { get; set; } = false;
        public int? SequenceOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}