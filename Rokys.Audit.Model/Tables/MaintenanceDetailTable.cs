namespace Rokys.Audit.Model.Tables
{
    public class MaintenanceDetailTable : AuditEntity
    {
        public Guid MaintenanceDetailTableId { get; set; } = Guid.NewGuid();
        public Guid MaintenanceTableId { get; set; }
        public string Code { get; set; } = default!;
        public string? Description { get; set; }
        public string? JsonData { get; set; }
        public int? OrderRow { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }

        // Relación
        public MaintenanceTable MaintenanceTable { get; set; } = default!;
    }
}
