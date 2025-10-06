using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.MaintenanceDetailTable
{
    public class MaintenanceDetailTableResponseDto : MaintenanceDetailTableDto
    {
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}