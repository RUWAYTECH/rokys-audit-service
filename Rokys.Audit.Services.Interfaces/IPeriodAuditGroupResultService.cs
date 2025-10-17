using Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.Common;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IPeriodAuditGroupResultService : IBaseService<PeriodAuditGroupResultRequestDto, PeriodAuditGroupResultResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<PeriodAuditGroupResultResponseDto>>> GetPaged(PeriodAuditGroupResultFilterRequestDto filterRequestDto);
        Task<ResponseDto<bool>> Recalculate(Guid periodAuditGroupResultId);
    }
}
