namespace Rokys.Audit.DTOs.Common
{
    public class ScaleGroupDto
    {
        public Guid GroupId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Weighting { get; set; }
    }
}
