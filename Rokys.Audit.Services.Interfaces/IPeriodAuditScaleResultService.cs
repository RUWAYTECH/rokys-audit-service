using Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IPeriodAuditScaleResultService : IBaseService<PeriodAuditScaleResultRequestDto, PeriodAuditScaleResultResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<PeriodAuditScaleResultResponseDto>>> GetPaged(PeriodAuditScaleResultFilterRequestDto filter);
        Task<ResponseDto<PeriodAuditScaleResultCustomResponseDto>> GetByIdCustomData(Guid id);
        Task<ResponseDto<bool>> UpdateAllFieldValues(Guid periodAuditGroupResultId, PeriodAuditFieldValuesUpdateAllValuesRequestDto periodAuditFieldValuesUpdateAllValuesRequestDto);
    }
}
