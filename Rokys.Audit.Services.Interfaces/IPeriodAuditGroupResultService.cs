using Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.Common;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IPeriodAuditGroupResultService
    {
        Task<ResponseDto<PeriodAuditGroupResultResponseDto>> Create(PeriodAuditGroupResultRequestDto requestDto);
        Task<ResponseDto<PeriodAuditGroupResultResponseDto>> Update(Guid id, PeriodAuditGroupResultRequestDto requestDto);
        Task<ResponseDto> Delete(Guid id);
        Task<ResponseDto<PeriodAuditGroupResultResponseDto>> GetById(Guid id);
        Task<ResponseDto<PaginationResponseDto<PeriodAuditGroupResultResponseDto>>> GetPaged(PeriodAuditGroupResultFilterRequestDto filterRequestDto);
    }
}
