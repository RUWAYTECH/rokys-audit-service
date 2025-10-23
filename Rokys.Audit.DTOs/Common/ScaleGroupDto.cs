namespace Rokys.Audit.DTOs.Common
{
    public class ScaleGroupDto
    {
        public Guid GroupId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool? HasSourceData { get; set; }
        public decimal Weighting { get; set; }
        public int SortOrder { get; set; } = 0;
    }
}
