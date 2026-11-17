namespace Rokys.Audit.DTOs.Common
{
    public class ScaleCompanyDto
    {
        public Guid? EnterpriseGroupingId { get; set; }
        public Guid? EnterpriseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string? ColorCode { get; set; }
        public decimal? NormalizedScore { get; set; }
        public decimal? ExpectedDistribution { get; set; }
        public int LevelOrder { get; set; }
    }
}
