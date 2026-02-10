namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataByAuditableGroupResponseDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
        public AuditablePointsResponseDto[] AuditablePoints { get; set; } = [];
    }

    public class AuditablePointsResponseDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Average { get; set; }
        public int AuditCount { get; set; }
    }
}
