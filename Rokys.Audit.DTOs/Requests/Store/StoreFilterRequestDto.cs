using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.Store
{
    public class StoreFilterRequestDto : PaginationRequestDto
    {
        public Guid? EnterpriseId { get; set; }
    }
}
