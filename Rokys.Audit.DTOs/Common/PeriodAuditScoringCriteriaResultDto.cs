namespace Rokys.Audit.DTOs.Common
{
    public class PeriodAuditScoringCriteriaResultDto
    {
        public Guid PeriodAuditScaleResultId { get; set; }
        public string? CriteriaName { get; set; }
        public string? CriteriaCode { get; set; }
        public string? ResultFormula { get; set; }
        public string? ComparisonOperator { get; set; }
        public string? ExpectedValue { get; set; }
        public decimal? Score { get; set; }
        public int? SortOrder { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public decimal? ResultObtained { get; set; }
    }
}
