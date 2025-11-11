using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.AuditRoleConfiguration;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IAuditRoleConfigurationService
    {
        Task<ResponseDto<List<AuditRoleConfigurationResponseDto>>> GetActiveConfigurationsAsync();
    }
}