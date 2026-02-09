using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.Common;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IKPIReportsService
    {
        Task<ResponseDto<object>> GetGeneralKPIsAsync(int year, Guid[] enterpriseIds, Guid? enterpriseGroupingId);
    }
}
