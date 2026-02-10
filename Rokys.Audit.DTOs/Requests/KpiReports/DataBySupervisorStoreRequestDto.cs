namespace Rokys.Audit.DTOs.Requests.KpiReports
{
    public class DataBySupervisorStoreRequestDto
    {
        public required Guid EnterpriseGroupingId { get; set; }
        public Guid[]? EnterpriseIds { get; set; }
        public Guid[]? StoreIds { get; set; }
        public Guid[]? SupervisorIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
