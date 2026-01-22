using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.Enterprise
{
    public class EnterpriseFilterRequestDto : PaginationRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
