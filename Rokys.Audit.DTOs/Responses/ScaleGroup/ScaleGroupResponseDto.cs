using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.ScaleGroup
{
    public class ScaleGroupResponseDto : ScaleGroupDto
    {
        public Guid ScaleGroupId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
    }
}
