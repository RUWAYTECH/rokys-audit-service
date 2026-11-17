using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.SubScale
{
    public class SubScaleFilterRequestDto : PaginationRequestDto
    {
        public Guid? EnterpriseGroupingId { get; set; }
    }
}
