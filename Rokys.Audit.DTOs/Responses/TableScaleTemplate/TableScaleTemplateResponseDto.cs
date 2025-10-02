namespace Rokys.Audit.DTOs.Responses.TableScaleTemplate
{
    public class TableScaleTemplateResponseDto
    {
        public Guid TableScaleTemplateId { get; set; }
        public Guid ScaleGroupId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? TemplateData { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public string? ScaleGroupName { get; set; }

        // Audit properties
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}