namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataByParticipantResponseDto
    {
        public string StoreId { get; set; } = string.Empty;
        public string Store { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public string UserReferenceId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public decimal Average { get; set; }
    }
}
