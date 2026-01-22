namespace Rokys.Audit.DTOs.Common
{
    public class SystemConfigurationDto
    {
        public string ConfigKey { get; set; } = string.Empty;
        public string? ConfigValue { get; set; }
        public string? DataType { get; set; }
        public string? Description { get; set; }
        public string? ReferenceType { get; set; }
        public string? ReferenceCode { get; set; }
    }
}
