namespace Rokys.Audit.DTOs.Requests.KpiReports
{
  public class DataByParticipantRequestDto
  {
    public required Guid EnterpriseGroupingId { get; set; }
    public Guid[]? EnterpriseIds { get; set; }
    public Guid[]? StoreIds { get; set; }
    public required string ParticipantType { get; set; } // A001,A002,A003 ETC  
    public Guid[]? UserReferenceIds { get; set; }
    public string? Months { get; set; } = string.Empty; // Formato: "01,02,03" para filtrar por meses específicos (opcional)
  }
}
