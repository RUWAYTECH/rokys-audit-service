namespace Rokys.Audit.Model.Tables
{
    public class GroupingUser : AuditEntity
    {
        public Guid GroupingUserId { get; set; } = Guid.NewGuid();

        public Guid EnterpriseGroupingId { get; set; }
        public Guid UserReferenceId { get; set; }

        public string RolesCodes { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual EnterpriseGrouping EnterpriseGrouping { get; set; } = null!;
        public virtual UserReference UserReference { get; set; } = null!;
    }
}
