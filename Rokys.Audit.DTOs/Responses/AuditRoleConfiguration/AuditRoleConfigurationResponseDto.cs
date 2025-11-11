namespace Rokys.Audit.DTOs.Responses.AuditRoleConfiguration
{
    public class AuditRoleConfigurationResponseDto
    {
        public Guid AuditRoleConfigurationId { get; set; }
        public string RoleCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public bool AllowMultiple { get; set; }
        public int? SequenceOrder { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}