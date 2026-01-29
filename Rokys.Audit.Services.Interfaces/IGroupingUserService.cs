using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.GroupingUser;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.GroupingUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IGroupingUserService : IBaseService<GroupingUserRequestDto, GroupingUserResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<GroupingUserResponseDto>>> GetPaged(GroupingUserFilterRequestDto filterRequest);
    }
}
