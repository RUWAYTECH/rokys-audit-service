using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.AuditTemplateField
{
    public class AuditTemplateFieldResponseDto : AuditTemplateFieldDto
    {
        public Guid AuditTemplateFieldId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string? TableScaleTemplateName { get; set; }
    }
}
