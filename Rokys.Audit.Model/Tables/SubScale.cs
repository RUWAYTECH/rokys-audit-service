namespace Rokys.Audit.Model.Tables
{
    public class SubScale : AuditEntity
    {
        public Guid SubScaleId { get; set; } = Guid.NewGuid();
        public Guid EnterpriseGroupingId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual EnterpriseGrouping EnterpriseGrouping { get; set; } = null!;
    }
}
