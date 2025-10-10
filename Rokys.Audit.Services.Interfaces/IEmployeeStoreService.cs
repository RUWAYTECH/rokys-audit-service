using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.EmployeeStore;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.EmployeeStore;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IEmployeeStoreService : IBaseService<EmployeeStoreRequestDto, EmployeeStoreResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<EmployeeStoreResponseDto>>> GetPaged(PaginationRequestDto requestDto);
        Task<ResponseDto<List<EmployeeStoreResponseDto>>> GetByUserReferenceId(Guid UserReferenceId);
    }
}
