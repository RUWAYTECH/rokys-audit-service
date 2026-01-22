using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.DTOs.Requests.SystemConfiguration
{
    public class SystemConfigurationRequestDto : SystemConfigurationDto
    {
        public bool IsActive { get; set; }
    }
}
