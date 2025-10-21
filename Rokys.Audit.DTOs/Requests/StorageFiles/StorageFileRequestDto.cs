using Microsoft.AspNetCore.Http;

namespace Rokys.Audit.DTOs.Requests.StorageFiles
{
    public class StorageFileRequestDto
    {
        public Guid EntityId { get; set; }
        public string? EntityName { get; set; }
        public string? ClassificationType { get; set; }
        public IFormFile? File { get; set; }
    }
}
