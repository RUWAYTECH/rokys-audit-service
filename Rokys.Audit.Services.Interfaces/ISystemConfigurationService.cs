using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.SystemConfiguration;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.SystemConfiguration;

namespace Rokys.Audit.Services.Interfaces
{
    public interface ISystemConfigurationService : IBaseService<SystemConfigurationRequestDto, SystemConfigurationResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<SystemConfigurationResponseDto>>> GetPaged(SystemConfigurationFilterRequestDto requestDto);
    }
}
