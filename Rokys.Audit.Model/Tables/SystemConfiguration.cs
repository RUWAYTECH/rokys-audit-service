namespace Rokys.Audit.Model.Tables
{
    public class SystemConfiguration : AuditEntity
    {
        public Guid SystemConfigurationId { get; set; } = Guid.NewGuid();
        public string ConfigKey { get; set; } = string.Empty;
        public string? ConfigValue { get; set; }
        public string? DataType { get; set; }
        public string? Description { get; set; }
        public string? ReferenceType { get; set; }
        public string? ReferenceCode { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
