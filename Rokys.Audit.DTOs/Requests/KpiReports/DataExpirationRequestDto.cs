namespace Rokys.Audit.DTOs.Requests.KpiReports
{
  public class DataExpirationRequestDto
  {
    public required Guid EnterpriseGroupingId { get; set; }
    public Guid[]? EnterpriseIds { get; set; }
    public Guid[]? StoreIds { get; set; }
    public Guid[]? SupervisorIds { get; set; }
    public Guid[]? AuditorIds { get; set; }
    public string? Months { get; set; } = string.Empty; // Formato: "01,02,03" para filtrar por meses específicos (opcional)
  }
}
