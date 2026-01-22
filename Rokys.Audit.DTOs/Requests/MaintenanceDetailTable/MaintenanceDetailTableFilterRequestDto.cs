using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.MaintenanceDetailTable
{
    public class MaintenanceDetailTableFilterRequestDto : PaginationRequestDto
    {
        public string? MaintenanceTableCode { get; set; }
    }
}
