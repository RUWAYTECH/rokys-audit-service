using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.SubScale;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.SubScale;

namespace Rokys.Audit.Services.Interfaces
{
    public interface ISubScaleService : IBaseService<SubScaleRequestDto, SubScaleResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<SubScaleResponseDto>>> GetPaged(SubScaleFilterRequestDto requestDto);
    }
}
