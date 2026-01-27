using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.AuditRoleConfiguration
{
    public class AuditRoleConfigurationFilterRequestDto : PaginationRequestDto
    {
        public Guid? EnterpriseId { get; set; }

        public Guid? EnterpriseGroupingId { get; set; }
    }
}