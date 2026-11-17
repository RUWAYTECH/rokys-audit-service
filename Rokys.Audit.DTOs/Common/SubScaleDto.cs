namespace Rokys.Audit.DTOs.Common
{
    public class SubScaleDto
    {
        public Guid EnterpriseGroupingId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string ColorCode { get; set; }
    }
}
