using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.StorageFiles
{
    public class StorageFileFilterRequestDto : PaginationRequestDto
    {
        public Guid? EntityId { get; set; }
        public string? EntityName { get; set; }
        public string? FileName { get; set; }
        public bool? IsActive { get; set; }
        public string? ClassificationType { get; set; }
    }
}
