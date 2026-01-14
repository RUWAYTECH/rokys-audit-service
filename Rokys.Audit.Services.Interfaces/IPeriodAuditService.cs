using Rokys.Audit.DTOs.Requests.PeriodAudit;
using Rokys.Audit.DTOs.Responses.PeriodAudit;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Common;
using System;
using System.Threading.Tasks;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IPeriodAuditService : IBaseService<PeriodAuditRequestDto, PeriodAuditResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<PeriodAuditResponseDto>>> GetPaged(PeriodAuditFilterRequestDto requestDto);
        Task<ResponseDto<bool>> Recalculate(Guid periodAuditId);
    Task<ResponseDto> ProcessAction(Rokys.Audit.DTOs.Requests.PeriodAudit.PeriodAuditBatchActionRequestDto requestDto);
        Task<ResponseDto<LastAuditByStoreIdResponseDto>> GetLasAuditByStoreId(Guid storeId);
        Task<ResponseDto<PeriodAuditReportResponseDto>> Export(PeriodAuditFilterRequestDto requestDto);
    }
}