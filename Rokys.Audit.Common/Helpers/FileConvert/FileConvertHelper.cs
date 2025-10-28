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
                "jpg" => "image/jpeg",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "xls" => "application/vnd.ms-excel",
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

            // Detectar XLS (formato binario antiguo)
            if (fileBytes.Length >= 4 &&
                fileBytes[0] == 0xD0 && fileBytes[1] == 0xCF && fileBytes[2] == 0x11 && fileBytes[3] == 0xE0)
            {
                extension = "xls";
                mimeType = "application/vnd.ms-excel";
            }
            // Detectar XLSX (ZIP structure, puede variar pero siempre empieza con PK)
            else if (fileBytes.Length >= 2 &&
                     fileBytes[0] == 0x50 && fileBytes[1] == 0x4B)
            {
                extension = "xlsx";
                mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            // Detectar DOC / DOCX (opcional)
            else if (fileBytes.Length >= 4 &&
                     fileBytes[0] == 0x25 && fileBytes[1] == 0x50) // PDF
            {
                extension = "pdf";
                mimeType = "application/pdf";
            }
            else
            {
                extension = "bin";
                mimeType = "application/octet-stream";
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
