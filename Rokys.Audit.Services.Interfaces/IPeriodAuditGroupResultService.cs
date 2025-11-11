using Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.ScaleGroup;
using Rokys.Audit.DTOs.Responses.PeriodAuditScaleResult;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IPeriodAuditGroupResultService : IBaseService<PeriodAuditGroupResultRequestDto, PeriodAuditGroupResultResponseDto>
    {
        public Task<ResponseDto<PeriodAuditGroupResultResponseDto>> Create(PeriodAuditGroupResultRequestDto requestDto, bool isTrasacction = false);
        Task<ResponseDto<PaginationResponseDto<PeriodAuditGroupResultResponseDto>>> GetPaged(PeriodAuditGroupResultFilterRequestDto filterRequestDto);
        Task<ResponseDto<bool>> Recalculate(Guid periodAuditGroupResultId);
        Task<ResponseDto<PeriodAuditGroupResultResponseDto>> Update(Guid id, UpdatePeriodAuditGroupResultRequestDto requestDto);
        Task CreateTableScaleTemplateResults(ScaleGroupResponseDto scale, PeriodAuditScaleResultResponseDto periodAuditScaleResult);
    }
}
