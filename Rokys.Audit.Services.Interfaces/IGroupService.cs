using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Group;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Group;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IGroupService : IBaseService<GroupRequestDto, GroupResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<GroupResponseDto>>> GetPaged(PaginationRequestDto requestDto);
    }
}