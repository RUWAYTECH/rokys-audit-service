namespace Rokys.Audit.DTOs.Common
{
    public class StoreDto
    {
        public string Name { get; set; }
        public string? Code { get; set; }
        public string Address { get; set; }
        public Guid EnterpriseId { get; set; }
    }
}
