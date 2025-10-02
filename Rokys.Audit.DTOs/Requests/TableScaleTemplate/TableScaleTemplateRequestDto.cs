using System.ComponentModel.DataAnnotations;

namespace Rokys.Audit.DTOs.Requests.TableScaleTemplate
{
    public class TableScaleTemplateRequestDto
    {
        [Required(ErrorMessage = "El ID del grupo de escala es requerido")]
        public Guid ScaleGroupId { get; set; }

        [Required(ErrorMessage = "El código es requerido")]
        [StringLength(50, ErrorMessage = "El código no puede exceder los 50 caracteres")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "El título no puede exceder los 255 caracteres")]
        public string? Title { get; set; }

        public string? TemplateData { get; set; }

        public bool IsActive { get; set; } = true;
    }
}