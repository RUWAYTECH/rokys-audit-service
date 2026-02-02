using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.UserReference
{
    public class UserReferenceFilterEnterpriseRequestDto : PaginationRequestDto
    {
        public string? RoleCode { get; set; }
    }
}
