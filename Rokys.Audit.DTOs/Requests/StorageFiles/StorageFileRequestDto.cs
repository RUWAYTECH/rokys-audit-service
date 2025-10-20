namespace Rokys.Audit.DTOs.Requests.StorageFiles
{
    public class StorageFileRequestDto
    {
        public string OriginalName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public string? UploadedBy { get; set; }
        public DateTime? UploadDate { get; set; }
        public string? ClassificationType { get; set; }
        public Guid EntityId { get; set; }
        public string? EntityName { get; set; }
        public bool? IsActive { get; set; }
    }
}
