namespace Rokys.Audit.DTOs.Requests.SubScale
{
    public class SubScaleRequestDto
    {
        public Guid EnterpriseGroupingId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string ColorCode { get; set; }
    }
}
