using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.ScoringCriteria
{
    public class ScoringCriteriaResponseDto : ScoringCriteriaDto
    {
        public Guid ScoringCriteriaId { get; set; }
        public string? CriteriaCode { get; set; }
        public string? CriteriaName { get; set; }
        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? ScaleGroupName { get; set; }
        public string? ScaleCalificationDescription { get; set; }
    }
}
