namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataByParticipantResponseDto
    {
        public string UserReferenceId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
        public MonthlyParticipantDataDto[] Months { get; set; } = Array.Empty<MonthlyParticipantDataDto>();
    }

    public class MonthlyParticipantDataDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
    }
}
