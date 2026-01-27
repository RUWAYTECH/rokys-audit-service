using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.AuditRoleConfiguration
{
    public class AuditRoleConfigurationResponseDto : AuditRoleConfigurationDto
    {
        public Guid AuditRoleConfigurationId { get; set; }                        
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string EnterpriseName { get; set; }
    }
}