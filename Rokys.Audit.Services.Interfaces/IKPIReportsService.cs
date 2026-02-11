using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.KpiReports;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.KpiReports;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IKPIReportsService
    {
        Task<ResponseDto<object>> GetGeneralKPIsAsync(int year, Guid[] enterpriseIds, Guid? enterpriseGroupingId);
        Task<ResponseDto<List<DataByParticipantResponseDto>>> GetDataByParticipantAsync(DataByParticipantRequestDto request);
        Task<ResponseDto<List<DataByAuditableGroupResponseDto>>> GetDataByAuditableGroupAsync(DataByAuditableGroupRequestDto request);
        Task<ResponseDto<List<DataBySupervisorStoreResponseDto>>> GetDataBySupervisorStoreAsync(DataBySupervisorStoreRequestDto request);
        Task<ResponseDto<List<DataByStoreResponseDto>>> GetDataByStoreAsync(DataByStoreRequestDto request);
        Task<ResponseDto<List<DataByScaleResponseDto>>> GetDataByScaleAsync(DataByScaleRequestDto request);
        Task<ResponseDto<List<DataExpirationResponseDto>>> GetExpiredProductsAsync(DataExpirationRequestDto request);
    }
}

