using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.ScaleCompany
{
    public class ScaleCompanyFilterRequestDto : PaginationRequestDto
    {
        public Guid? EnterpriseId { get; set; }
    }
}
