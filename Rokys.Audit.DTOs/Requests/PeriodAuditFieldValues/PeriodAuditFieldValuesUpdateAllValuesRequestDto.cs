namespace Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues
{
    public class  PeriodAuditFieldValuesUpdateAllValuesRequestDto
    {
        public Guid PeriodAuditScoringCriteriaResultId { get; set; }
        public decimal ResultObtained { get; set; }
        public List<periodAuditScaleSubResult> PeriodAuditScaleSubResult { get; set; }
        public List<UpdatePeriodAuditFieldValuesRequestDto> PeriodAuditFieldValues { get; set; }
    }

    public class periodAuditScaleSubResult
    {
        public Guid PeriodAuditScaleSubResultId { get; set; }
        public decimal ScoreObtained { get; set; }
    }
}
