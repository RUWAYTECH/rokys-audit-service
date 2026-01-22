using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.MaintenanceTable;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.MaintenanceTable;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IMaintenanceTableService : IBaseService<MaintenanceTableRequestDto, MaintenanceTableResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<MaintenanceTableResponseDto>>> GetPaged(MaintenanceTableFilterRequestDto requestDto);
    }
}
