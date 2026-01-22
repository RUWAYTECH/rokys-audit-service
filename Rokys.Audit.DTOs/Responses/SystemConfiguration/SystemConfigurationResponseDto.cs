using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Responses.SystemConfiguration
{
    public class SystemConfigurationResponseDto : SystemConfigurationDto
    {
        public Guid SystemConfigurationId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
    }
}
