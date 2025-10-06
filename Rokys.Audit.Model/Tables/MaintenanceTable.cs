namespace Rokys.Audit.Model.Tables
{
    public class MaintenanceTable : AuditEntity
    {
        public Guid MaintenanceTableId { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; }

        // Relaciones
        public ICollection<MaintenanceDetailTable>? Details { get; set; }
    }
}
