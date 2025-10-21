using Rokys.Audit.DTOs.Requests.InboxItems;
using Rokys.Audit.DTOs.Responses.InboxItems;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IInboxItemsService : IBaseService<InboxItemRequestDto, InboxItemResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<InboxItemResponseDto>>> GetPaged(InboxItemFilterRequestDto requestDto);
    }
}
