using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.AuditRoleConfiguration;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.AuditRoleConfiguration;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IAuditRoleConfigurationService : IBaseService<AuditRoleConfigurationRequestDto, AuditRoleConfigurationResponseDto>
    {
        Task<ResponseDto<List<AuditRoleConfigurationResponseDto>>> GetActiveConfigurationsAsync();
        Task<ResponseDto<PaginationResponseDto<AuditRoleConfigurationResponseDto>>> GetPaged(AuditRoleConfigurationFilterRequestDto requestDto);
        Task<ResponseDto<bool>> ChangeOrder(int currentPosition, int newPosition, Guid? enterpriseId);
    }
}