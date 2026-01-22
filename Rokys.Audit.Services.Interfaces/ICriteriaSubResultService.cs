
using Rokys.Audit.DTOs.Requests.CriteriaSubResult;
using Rokys.Audit.DTOs.Responses.CriteriaSubResult;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.Services.Interfaces
{
    public interface ICriteriaSubResultService : IBaseService<CriteriaSubResultRequestDto, CriteriaSubResultResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<CriteriaSubResultResponseDto>>> GetPaged(CriteriaSubResultFilterRequestDto requestDto);
    }
}
