using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.ScaleCompany
{
    public class ScaleCompanyFilterRequestDto : PaginationRequestDto
    {
        public Guid? EnterpriseGroupingId { get; set; }
        public Guid? EnterpriseId { get; set; }
    }
}
