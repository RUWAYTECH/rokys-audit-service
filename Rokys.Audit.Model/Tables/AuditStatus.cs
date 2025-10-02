namespace Rokys.Audit.Model.Tables
{
    public class AuditStatus : AuditEntity
    {
        public Guid AuditStatusId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? ColorCode { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<PeriodAudit> PeriodAudits { get; set; } = new List<PeriodAudit>();
    }
}