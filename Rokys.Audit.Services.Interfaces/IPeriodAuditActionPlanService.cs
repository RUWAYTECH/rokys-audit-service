using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.PeriodAuditActionPlan;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IPeriodAuditActionPlanService
    {
        Task<ResponseDto<EnterpriseConfigurationResponseDto>> GetEnterpriseConfigurationByPeriodAuditId(Guid periodAuditId);
    }
}
