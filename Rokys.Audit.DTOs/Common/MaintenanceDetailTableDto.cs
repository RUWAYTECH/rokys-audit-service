using System;

namespace Rokys.Audit.DTOs.Common
{
    public class MaintenanceDetailTableDto
    {
        public Guid MaintenanceTableId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? JsonData { get; set; }
        public int? OrderRow { get; set; }
        public bool IsDefault { get; set; }
    }
}