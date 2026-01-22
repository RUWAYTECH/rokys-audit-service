using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.ScaleGroup
{
    public class ScaleGroupFilterRequestDto : PaginationRequestDto
    {
        public Guid? GroupId { get; set; }
    }
}
