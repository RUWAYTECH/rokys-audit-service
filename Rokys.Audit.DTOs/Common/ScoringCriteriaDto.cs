namespace Rokys.Audit.DTOs.Common
{
    public class ScoringCriteriaDto
    {
        public Guid ScaleGroupId { get; set; }
        public string CriteriaName { get; set; } = string.Empty;
        public string? CriteriaCode { get; set; }
        public string? ResultFormula { get; set; }
        public string ComparisonOperator { get; set; } = string.Empty;
        public string ExpectedValue { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public int SortOrder { get; set; } = 0;
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
