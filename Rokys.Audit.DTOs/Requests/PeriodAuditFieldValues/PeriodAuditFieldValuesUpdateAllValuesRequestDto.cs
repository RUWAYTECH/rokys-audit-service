namespace Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues
{
    public class  PeriodAuditFieldValuesUpdateAllValuesRequestDto
    {
        public Guid ScoringCriteriaId { get; set; }
        public decimal ResultObtained { get; set; }
        public List<UpdatePeriodAuditFieldValuesRequestDto> PeriodAuditFieldValues { get; set; }
    }
}
