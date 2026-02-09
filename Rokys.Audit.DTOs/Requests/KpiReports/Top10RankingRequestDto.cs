namespace Rokys.Audit.DTOs.Requests.KpiReports
{
    public class TopRankingRequestDto
    {
        public required Guid EnterpriseGroupingId { get; set; }
        public Guid[]? EnterpriseIds { get; set; }
        public Guid[]? SupervisorIds { get; set; }
        public Guid[]? AuditorIds { get; set; }
        public string? Months { get; set; } // "01,02,03,04" etc.
        public required string RankingType { get; set; } // "best" o "risk"
        public int TopCount { get; set; } = 10; // Cantidad de registros a devolver
    }
}
