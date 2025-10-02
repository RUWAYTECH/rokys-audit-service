using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.ScaleGroup;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.ScaleGroup;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IScaleGroupService : IBaseService<ScaleGroupRequestDto, ScaleGroupResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<ScaleGroupResponseDto>>> GetPaged(PaginationRequestDto requestDto);
        Task<ResponseDto<IEnumerable<ScaleGroupResponseDto>>> GetByGroupId(Guid groupId);
    }
}