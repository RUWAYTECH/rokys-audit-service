using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.EnterpriseGrouping;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.EnterpriseGrouping;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IEnterpriseGroupingService : IBaseService<EnterpriseGroupingRequestDto, EnterpriseGroupingResponseDto>
    {
        Task<ResponseDto<EnterpriseGroupingResponseDto>> Create(EnterpriseGroupingCreateRequestDto requestDto);
        Task<ResponseDto<PaginationResponseDto<EnterpriseGroupingResponseDto>>> GetPaged(EnterpriseGroupingFilterRequestDto requestDto);
        Task<ResponseDto> DeleteEnterpriseGroupById(Guid id);
    }
}
