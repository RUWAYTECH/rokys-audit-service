namespace Rokys.Audit.DTOs.Common
{
    public class CriteriaSubResultDto
    {
        public Guid ScaleGroupId { get; set; }
        public string CriteriaName { get; set; } = string.Empty;
        public string? CriteriaCode { get; set; }
        public string? ResultFormula { get; set; }
        public string ColorCode { get; set; } = string.Empty;
        public decimal? Score { get; set; }
    }
}
