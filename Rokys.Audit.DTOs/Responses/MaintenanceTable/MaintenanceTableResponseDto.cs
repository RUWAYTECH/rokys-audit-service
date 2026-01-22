using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.MaintenanceTable
{
    public class MaintenanceTableResponseDto : MaintenanceTableDto
    {
        public Guid MaintenanceTableId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = default!;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
