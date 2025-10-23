using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.ScaleGroup;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.ScaleGroup;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IScaleGroupService : IBaseService<ScaleGroupRequestDto, ScaleGroupResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<ScaleGroupResponseDto>>> GetPaged(ScaleGroupFilterRequestDto requestDto);
        Task<ResponseDto<IEnumerable<ScaleGroupResponseDto>>> GetByGroupId(Guid groupId);
        Task<ResponseDto<bool>> ChangeOrder(Guid groupId, int currentPosition, int newPosition);
    }
}