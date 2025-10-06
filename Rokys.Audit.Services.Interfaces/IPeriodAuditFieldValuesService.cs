using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;
using Rokys.Audit.DTOs.Responses.PeriodAuditFieldValues;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Common;
using System;
using System.Threading.Tasks;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IPeriodAuditFieldValuesService : IBaseService<PeriodAuditFieldValuesRequestDto, PeriodAuditFieldValuesResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<PeriodAuditFieldValuesResponseDto>>> GetPaged(PaginationRequestDto requestDto);
    }
}