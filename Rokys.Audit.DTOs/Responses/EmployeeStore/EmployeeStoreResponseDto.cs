using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.EmployeeStore
{
    public class EmployeeStoreResponseDto : EmployeeStoreDto
    {
        public Guid EmployeeStoreId { get; set; }
        public bool IsActive { get; set; }
    }
}
