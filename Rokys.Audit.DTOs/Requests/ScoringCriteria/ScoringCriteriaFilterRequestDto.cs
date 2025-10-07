using System;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.ScoringCriteria
{
    public class ScoringCriteriaFilterRequestDto : PaginationRequestDto
    {
        public Guid? ScaleGroupId { get; set; }
    }
}
