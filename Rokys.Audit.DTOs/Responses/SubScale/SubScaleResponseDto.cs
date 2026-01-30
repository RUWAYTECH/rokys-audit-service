namespace Rokys.Audit.DTOs.Responses.SubScale
{
    public class SubScaleResponseDto
    {
        public Guid SubScaleId { get; set; }
        public Guid EnterpriseGroupingId { get; set; }
        public string? EnterpriseGroupingName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string ColorCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
