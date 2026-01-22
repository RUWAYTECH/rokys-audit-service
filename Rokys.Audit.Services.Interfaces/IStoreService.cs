using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Store;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Store;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IStoreService : IBaseService<StoreRequestDto, StoreResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<StoreResponseDto>>> GetPaged(StoreFilterRequestDto requestDto);
    }
}
