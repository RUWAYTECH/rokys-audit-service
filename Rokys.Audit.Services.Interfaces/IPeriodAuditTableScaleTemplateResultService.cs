using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rokys.Audit.DTOs.Requests.PeriodAuditTableScaleTemplateResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditTableScaleTemplateResult;
using Rokys.Audit.DTOs.Responses.Common;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IPeriodAuditTableScaleTemplateResultService
    {
        Task<ResponseDto<PeriodAuditTableScaleTemplateResultResponseDto>> Create(PeriodAuditTableScaleTemplateResultRequestDto requestDto);
        Task<ResponseDto<PeriodAuditTableScaleTemplateResultResponseDto>> Update(Guid id, PeriodAuditTableScaleTemplateResultRequestDto requestDto);
        Task<ResponseDto<bool>> Delete(Guid id);
        Task<ResponseDto<PeriodAuditTableScaleTemplateResultResponseDto>> GetById(Guid id);
        Task<ResponseDto<List<PeriodAuditTableScaleTemplateResultResponseDto>>> GetAll();
        Task<ResponseDto<List<PeriodAuditTableScaleTemplateResultResponseDto>>> Filter(PeriodAuditTableScaleTemplateResultFilterRequestDto filter);
    }
}
