using static Rokys.Audit.Common.Constant.Constants;

namespace Rokys.Audit.Common.Helpers.FileConvert
{
    public class FileBase64Result
    {
        public string Base64 { get; set; }
        public string MimeType { get; set; }
        public string Extension { get; set; }
    }
    public static class FileConvertHelper
    {


        /// <summary>
        /// Convierte un archivo de una ruta física a base64 y retorna base64, tipo MIME y extensión
        /// </summary>
        public static FileBase64Result FileToBase64Info(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return null;
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var base64 = Convert.ToBase64String(fileBytes);
            var extension = System.IO.Path.GetExtension(filePath)?.TrimStart('.').ToLower();
            var mimeType = GetMimeTypeByExtension(extension);
            return new FileBase64Result
            {
                Base64 = base64,
                MimeType = mimeType,
                Extension = extension
            };
        }

        public static FileBase64Result? GetFileBase64Result(string basePath, string filePath, string subFolder = "")
        {
            if (string.IsNullOrEmpty(filePath))
                return null;
            var fullPath = Path.Combine(basePath, FileDirectories.Uploads, subFolder, filePath);
            return FileToBase64Info(fullPath);
        }
        private static string GetMimeTypeByExtension(string extension)
        {
            return extension switch
            {
                "pdf" => "application/pdf",
                "doc" => "application/msword",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "jpg" => "image/jpeg",
                "jpeg" => "image/jpeg",
                "png" => "image/png",
                "gif" => "image/gif",
                "bmp" => "image/bmp",
                "txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
        /// <summary>
        /// Convierte un archivo de una ruta física a base64 (PDF, Word, imagen, etc.)
        /// </summary>
        public static string FileToBase64(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return null;
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return Convert.ToBase64String(fileBytes);
        }

        public static FileBase64Result FileBase64Result(string base64File)
        {
            if (string.IsNullOrEmpty(base64File))
                return null;

            var cleanBase64 = base64File.Contains(",") ? base64File.Split(',')[1] : base64File;

            var fileBytes = Convert.FromBase64String(cleanBase64);

            string extension;
            string mimeType;

            if (fileBytes.Length >= 4 && fileBytes[0] == 0xD0 && fileBytes[1] == 0xCF && fileBytes[2] == 0x11 && fileBytes[3] == 0xE0)
            {
                extension = "doc";
                mimeType = "application/msword";
            }
            else
            {
                extension = "docx";
                mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }

            return new FileBase64Result
            {
                Base64 = cleanBase64,
                Extension = extension,
                MimeType = mimeType
            };
        }

    }
}
