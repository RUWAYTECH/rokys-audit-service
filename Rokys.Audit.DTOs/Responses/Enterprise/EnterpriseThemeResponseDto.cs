namespace Rokys.Audit.DTOs.Responses.Enterprise
{
    class EnterpriseThemeResponseDto
    {
        public Guid EnterpriseThemeId { get; set; }
        public Guid EnterpriseId { get; set; }

        public string? LogoUrl { get; set; }
        public string? LogoFileName { get; set; }

        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public string? AccentColor { get; set; }
        public string? BackgroundColor { get; set; }
        public string? TextColor { get; set; }

        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
