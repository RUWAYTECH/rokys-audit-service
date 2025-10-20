namespace Rokys.Audit.DTOs.Responses.StorageFiles
{
    public class StorageFileResponseDto
    {
        public Guid StorageFileId { get; set; }
        public string OriginalName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public string? UploadedBy { get; set; }
        public DateTime UploadDate { get; set; }
        public string ClassificationType { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
