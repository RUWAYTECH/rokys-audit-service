namespace Rokys.Audit.DTOs.Common
{
    public class ScaleCompanyDto
    {
        public Guid EnterpriseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string? ColorCode { get; set; }
        public string? Icon { get; set; }
        public int SortOrder { get; set; }
    }
}
