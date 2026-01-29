using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.EnterpriseGrouping;
using Rokys.Audit.DTOs.Responses.UserReference;

namespace Rokys.Audit.DTOs.Responses.GroupingUser
{
    public class GroupingUserResponseDto : GroupingUserDto
    {
        public Guid GroupingUserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }
        public EnterpriseGroupingResponseDto EnterpriseGrouping { get; set; } = null!;
        public UserReferenceResponseDto UserReference { get; set; } = null!;
    }
}
