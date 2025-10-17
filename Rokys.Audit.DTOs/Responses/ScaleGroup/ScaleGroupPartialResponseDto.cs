using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.ScaleGroup
{
    public class ScaleGroupPartialResponseDto : ScaleGroupDto
    {
        public Guid ScaleGroupId { get; set; }
        public bool IsActive { get; set; }
    }
}
