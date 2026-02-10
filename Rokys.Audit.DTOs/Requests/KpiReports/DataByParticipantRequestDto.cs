namespace Rokys.Audit.DTOs.Requests.KpiReports
{
  public class DataByParticipantRequestDto
  {
    public required Guid EnterpriseGroupingId { get; set; }
    public Guid[]? EnterpriseIds { get; set; }
    public Guid[]? StoreIds { get; set; }
    public required string ParticipantType { get; set; } // A001,A002,A003 ETC  
    public Guid[]? SupervisorIds { get; set; }
    public Guid[]? AuditorIds { get; set; }
    public Guid[]? UnitManagerIds { get; set; }
    public string? Months { get; set; } = string.Empty; // Formato: "01,02,03" para filtrar por meses específicos (opcional)
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
  }
}
