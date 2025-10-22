using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.TableScaleTemplate
{
    public class TableScaleTemplateResponseDto: TableScaleTemplateDto
    {
        public Guid TableScaleTemplateId { get; set; }
       
        public bool IsActive { get; set; }

        // Navigation properties
        public string? ScaleGroupName { get; set; }
        public int SortOrder { get; set; }


        // Audit properties
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}