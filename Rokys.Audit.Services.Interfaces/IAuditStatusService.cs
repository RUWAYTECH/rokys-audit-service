using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.AuditStatus;
using Rokys.Audit.DTOs.Responses.AuditStatus;
using Rokys.Audit.DTOs.Responses.Common;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IAuditStatusService : IBaseService<AuditStatusRequestDto, AuditStatusResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<AuditStatusResponseDto>>> GetPaged(PaginationRequestDto requestDto);
    }
}
