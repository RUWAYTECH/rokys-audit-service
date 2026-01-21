namespace Rokys.Audit.DTOs.Requests.Enterprise
{
    public class EnterpriseThemeRequestDto
    {
        public string PrimaryColor { get; set; } = "#0066CC";
        public string SecondaryColor { get; set; } = "#333333";
        public string? AccentColor { get; set; }
        public string? BackgroundColor { get; set; }
        public string? TextColor { get; set; }
    }
}
