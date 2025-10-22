namespace Rokys.Audit.DTOs.Responses.StorageFiles
{
    public class StorageFileDownloadResponseDto
    {
        public Guid StorageFileId { get; set; }
        public string FileUrl { get; set; } = string.Empty; // stored filename or absolute path
        public string OriginalName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty; // stored filename
        public string? FileType { get; set; }
    }
}
