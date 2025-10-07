using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.ScoringCriteria;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.ScoringCriteria;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IScoringCriteriaService : IBaseService<ScoringCriteriaRequestDto, ScoringCriteriaResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<ScoringCriteriaResponseDto>>> GetPaged(ScoringCriteriaFilterRequestDto requestDto);
    }
}
