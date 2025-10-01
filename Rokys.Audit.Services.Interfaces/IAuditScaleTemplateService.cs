using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.AuditScaleTemplate;
using Rokys.Audit.DTOs.Responses.AuditScaleTemplate;
using Rokys.Audit.DTOs.Responses.Common;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IAuditScaleTemplateService : IBaseService<AuditScaleTemplateRequestDto, AuditScaleTemplateResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<AuditScaleTemplateResponseDto>>> GetPaged(PaginationRequestDto requestDto);
    }
}