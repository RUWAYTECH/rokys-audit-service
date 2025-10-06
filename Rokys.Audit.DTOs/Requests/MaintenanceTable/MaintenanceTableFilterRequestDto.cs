using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.MaintenanceTable
{
    public class MaintenanceTableFilterRequestDto : PaginationRequestDto
    {
        public string? Code { get; set; }
    }
}
