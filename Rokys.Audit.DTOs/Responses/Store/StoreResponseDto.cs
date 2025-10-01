using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.Store
{
    public class StoreResponseDto : StoreDto
    {
        public Guid StoreId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
    }
}
