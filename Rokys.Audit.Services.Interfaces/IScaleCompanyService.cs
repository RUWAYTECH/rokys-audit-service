using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.ScaleCompany;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.ScaleCompany;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IScaleCompanyService : IBaseService<ScaleCompanyRequestDto, ScaleCompanyResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<ScaleCompanyResponseDto>>> GetPaged(ScaleCompanyFilterRequestDto requestDto);
    }
}
