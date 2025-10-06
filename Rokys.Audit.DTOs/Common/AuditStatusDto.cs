namespace Rokys.Audit.DTOs.Common
{
    public class AuditStatusDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? ColorCode { get; set; }
        public bool IsActive { get; set; }
    }
}
