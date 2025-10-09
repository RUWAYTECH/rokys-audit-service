using Rokys.Audit.DTOs.Requests.MaintenanceDetailTable;
using Rokys.Audit.DTOs.Responses.MaintenanceDetailTable;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Common;
using System;
using System.Threading.Tasks;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IMaintenanceDetailTableService : IBaseService<MaintenanceDetailTableRequestDto, MaintenanceDetailTableResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<MaintenanceDetailTableResponseDto>>> GetPaged(MaintenanceDetailTableFilterRequestDto requestDto);
    }
}