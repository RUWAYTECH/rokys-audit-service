using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.ScoringCriteria
{
    public class ScoringCriteriaResponseDto : ScoringCriteriaDto
    {
        public Guid ScoringCriteriaId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
