namespace Rokys.Audit.DTOs.Common
{
    public class SubstitutionHistoryDto
    {
        public Guid PeriodAuditId { get; set; }
        public Guid PreviousUserReferenceId { get; set; }
        public Guid NewUserReferenceId { get; set; }
        public string? ChangeReason { get; set; }
    }
}
