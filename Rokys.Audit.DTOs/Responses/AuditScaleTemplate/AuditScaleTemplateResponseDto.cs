using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.AuditScaleTemplate
{
    public class AuditScaleTemplateResponseDto : AuditScaleTemplateDto
    {
        public Guid AuditScaleTemplateId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}