using Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IPeriodAuditScaleResultService : IBaseService<PeriodAuditScaleResultRequestDto, PeriodAuditScaleResultResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<PeriodAuditScaleResultResponseDto>>> GetPaged(PeriodAuditScaleResultFilterRequestDto filter);
        Task<ResponseDto<PeriodAuditScaleResultCustomResponseDto>> GetByIdCustomData(Guid id);
        Task<ResponseDto<bool>> UpdateAllFieldValues(Guid periodAuditGroupResultId, PeriodAuditFieldValuesUpdateAllValuesRequestDto periodAuditFieldValuesUpdateAllValuesRequestDto);
        public Task<ResponseDto<PeriodAuditScaleResultResponseDto>> Update(Guid id, UpdatePeriodAuditScaleResultRequestDto requestDto);

        /// <summary>
        /// Cambia el orden de un PeriodAuditScaleResult dentro de un PeriodAuditGroupResult
        /// </summary>
        /// <param name="periodAuditGroupResultId">ID del PeriodAuditGroupResult</param>
        /// <param name="currentPosition">Posición actual</param>
        /// <param name="newPosition">Nueva posición</param>
        /// <returns>Resultado del cambio de orden</returns>
        Task<ResponseDto<bool>> ChangeOrder(Guid periodAuditGroupResultId, int currentPosition, int newPosition);
    }
}
