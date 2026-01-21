using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Enterprise;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Enterprise;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IEnterpriseService : IBaseService<EnterpriseRequestDto, EnterpriseResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<EnterpriseResponseDto>>> GetPaged(EnterpriseFilterRequestDto requestDto);
        Task<ResponseDto<EnterpriseResponseDto>> Update(Guid id, EnterpriseUpdateRequestDto requestDto);
    }
}
