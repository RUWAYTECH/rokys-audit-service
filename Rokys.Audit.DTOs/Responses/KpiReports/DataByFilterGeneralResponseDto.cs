namespace Rokys.Audit.DTOs.Responses.KpiReports
{
    public class DataByFilterGeneralResponseDto
    {
        public int QuantityStores { get; set; }
        public int QuantityAudits { get; set; }
        public decimal OverallAverageScore { get; set; }
        public decimal PercentageEvaluations { get; set; }
    }
}
