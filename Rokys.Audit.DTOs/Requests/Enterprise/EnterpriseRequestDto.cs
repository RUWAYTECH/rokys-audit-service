using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.Enterprise
{
    public class EnterpriseRequestDto: EnterpriseDto
    {

        public bool IsActive { get; set; }
        // Theme properties - opcionales para crear/actualizar
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? AccentColor { get; set; }
        public string? BackgroundColor { get; set; }
        public string? TextColor { get; set; }

        // Logo properties - opcional
        public byte[]? LogoData { get; set; }
        public string? LogoContentType { get; set; }
        public string? LogoFileName { get; set; }
    }
}
