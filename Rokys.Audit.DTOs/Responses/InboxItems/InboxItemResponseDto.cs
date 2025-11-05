using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.AuditStatus;

namespace Rokys.Audit.DTOs.Responses.InboxItems
{
    public class InboxItemResponseDto : InboxItemDto
    {
        public Guid InboxItemId { get; set; }
        public string? UserName { get; set; }
        public AuditStatusResponseDto? NextStatus { get; set; }
        public AuditStatusResponseDto? PrevStatus { get; set; }
    }
}
