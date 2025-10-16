using System;

namespace Rokys.Audit.DTOs.Requests.ScoringCriteria
{
    public class ChangeScoringCriteriaOrderRequestDto
    {
        public Guid ScaleGroupId { get; set; }
        public int CurrentPosition { get; set; }
        public int NewPosition { get; set; }
    }
}
