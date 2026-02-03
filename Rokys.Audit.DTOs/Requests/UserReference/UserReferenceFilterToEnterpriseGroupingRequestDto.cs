using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.UserReference
{
    public class UserReferenceFilterToEnterpriseGroupingRequestDto : PaginationRequestDto
    {
        public string? Filter { get; set; }
    }
}
