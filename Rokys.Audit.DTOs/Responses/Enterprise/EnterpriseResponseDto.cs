using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.Enterprise
{
    public class EnterpriseResponseDto: EnterpriseDto
    {   
        public Guid EnterpriseId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public string AccentColor { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }

        public byte[]? LogoData { get; set; }
        public string? LogoContentType { get; set; }
        public string? LogoFileName { get; set; }
    }
}
