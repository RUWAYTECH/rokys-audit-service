using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.EnterpriseGroup;

namespace Rokys.Audit.DTOs.Responses.EnterpriseGrouping
{
    public class EnterpriseGroupingResponseDto : EnterpriseGroupingDto
    {
        public Guid EnterpriseGroupingId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public List<EnterpriseGroupResponseDto?> Groupings { get; set; }
    }
}
