namespace Rokys.Audit.DTOs.Common
{
    public class FileSettings
    {
        public long MaxFileSize { get; set; }
        public List<string> AllowedFileTypes { get; set; }
        public string Path { get; set; }
    }
}
