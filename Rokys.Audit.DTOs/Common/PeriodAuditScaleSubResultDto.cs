namespace Rokys.Audit.DTOs.Common
{
    public class PeriodAuditScaleSubResultDto
    {
        public Guid PeriodAuditScaleResultId { get; set; }
        public Guid CriteriaSubResultId { get; set; }
        public string? CriteriaCode { get; set; }
        public string? CriteriaName { get; set; }
        public string? EvaluatedValue { get; set; }
        public string? CalculatedResult { get; set; }
        public string? AppliedFormula { get; set; }
        public decimal? ScoreObtained { get; set; }
        public string? ColorCode { get; set; }
    }
}
