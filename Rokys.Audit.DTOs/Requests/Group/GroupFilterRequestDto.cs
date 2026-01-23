using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.Group
{
    public class GroupFilterRequestDto : PaginationRequestDto
    {
        public Guid? EnterpriseId { get; set; }
        public Guid? EnterpriseGroupingId { get; set; }
    }
}
