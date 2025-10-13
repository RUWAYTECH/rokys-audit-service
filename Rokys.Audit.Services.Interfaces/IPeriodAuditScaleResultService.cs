using Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IPeriodAuditScaleResultService
    {
        Task<ResponseDto<PeriodAuditScaleResultResponseDto>> Create(PeriodAuditScaleResultRequestDto requestDto);
        Task<ResponseDto<PeriodAuditScaleResultResponseDto>> Update(Guid id, PeriodAuditScaleResultRequestDto requestDto);
        Task<ResponseDto> Delete(Guid id);
        Task<ResponseDto<PeriodAuditScaleResultResponseDto>> GetById(Guid id);
        Task<ResponseDto<PaginationResponseDto<PeriodAuditScaleResultResponseDto>>> GetPaged(PeriodAuditScaleResultFilterRequestDto filter);
    }
}
