using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.GroupingUser;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.GroupingUser;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IGroupingUserService : IBaseService<GroupingUserRequestDto, GroupingUserResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<GroupingUserResponseDto>>> GetPaged(GroupingUserFilterRequestDto filterRequest);
    }
}
