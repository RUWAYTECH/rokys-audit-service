using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.EnterpriseGroup
{
    public class EnterpriseGroupResponseDto : EnterpriseGroupDto
    {
        public Guid EnterpriseGroupId { get; set; }
        public string? EnterpriseName { get; set; }
        public string? EnterpriseCode { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
    }
}
