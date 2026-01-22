namespace Rokys.Audit.Model.Tables
{
    public class Stores : AuditEntity
    {
        public Guid StoreId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Address { get; set; }
        public Guid EnterpriseId { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Enterprise Enterprise { get; set; } = null!;
        public virtual ICollection<PeriodAudit> PeriodAudits { get; set; } = new List<PeriodAudit>();
    }
}