namespace Rokys.Audit.Model.Tables
{
    public class EnterpriseGroup : AuditEntity
    {
        public Guid EnterpriseGroupId { get; set; } = Guid.NewGuid();
        public Guid EnterpriseId { get; set; }
        public Guid EnterpriseGroupingId { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Enterprise Enterprise { get; set; } = null!;
        public virtual EnterpriseGrouping EnterpriseGrouping { get; set; } = null!;
    }
}
