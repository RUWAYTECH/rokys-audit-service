namespace Rokys.Audit.DTOs.Common
{
    public class GroupDto
    {
        public Guid? EnterpriseId { get; set; }
        public Guid? EnterpriseGroupingId { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; }
        public decimal Weighting { get; set; }
        public decimal? NormalizedScore { get; set; }
        public decimal? ExpectedDistribution { get; set; }
        public int LevelOrder { get; set; } = 1;
        public string? ScaleType { get; set; }
    }
}

