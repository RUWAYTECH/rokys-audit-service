using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.AuditStatus
{
    public class AuditStatusResponseDto : AuditStatusDto
    {
        public Guid AuditStatusId { get; set; }
        public string CreatedBy { get; set; } = default!;
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
