namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataByParticipantResponseDto
    {
        public string UserReferenceId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
    }
}
