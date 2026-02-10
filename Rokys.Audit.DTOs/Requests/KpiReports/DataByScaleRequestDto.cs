namespace Rokys.Audit.DTOs.Requests.KpiReports
{
    public class DataByScaleRequestDto
    {
        public required Guid EnterpriseGroupingId { get; set; }
        public Guid[]? EnterpriseIds { get; set; }
        public Guid[]? StoreIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
