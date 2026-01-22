namespace Rokys.Audit.DTOs.Common
{
    public class AuditScaleTemplateDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TemplateData { get; set; } = string.Empty; // JSON almacenado como texto
    }
}