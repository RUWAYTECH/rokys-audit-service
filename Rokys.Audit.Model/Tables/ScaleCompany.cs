namespace Rokys.Audit.Model.Tables
{
    public class ScaleCompany : AuditEntity
    {
        public Guid ScaleCompanyId { get; set; } = Guid.NewGuid();
        public Guid EnterpriseId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string? ColorCode { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Enterprise Enterprise { get; set; } = null!;
    }
}
